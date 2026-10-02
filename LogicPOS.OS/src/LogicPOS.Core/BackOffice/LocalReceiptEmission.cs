using System.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class LocalReceiptEmission : IReceiptEmission
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IConfiguration _configuration;

    public LocalReceiptEmission(IServiceScopeFactory scopes, IConfiguration configuration)
    {
        _scopes = scopes;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<ReceiptEmissionRow>> ListAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var from = start.Date;
        var to = end.Date.AddDays(1);
        var documents = await database.Documents.AsNoTracking()
            .Include(document => document.Currency)
            .Where(document => document.IsDeleted == false
                && document.IsDraft == false
                && document.Paid == false
                && document.Status == "N"
                && (document.Type == "FT" || document.Type == "ND" || document.Type == "NC")
                && document.CreatedAt >= from
                && document.CreatedAt < to)
            .OrderByDescending(document => document.CreatedAt)
            .Take(500)
            .ToListAsync(cancellationToken);

        return documents.Select(document => new ReceiptEmissionRow
        {
            Id = document.Id,
            CustomerId = document.CustomerId,
            CurrencyId = document.CurrencyId,
            CreatedAt = document.CreatedAt,
            Number = document.Number,
            Status = document.Status,
            Type = document.Type,
            Paid = document.Paid,
            CustomerName = document.Customer.Name ?? string.Empty,
            FiscalNumber = document.Customer.FiscalNumber ?? string.Empty,
            Currency = document.Currency?.Designation ?? string.Empty,
            TotalFinal = document.TotalFinal,
            TotalPaid = 0m,
            TotalToPay = document.Type == "NC" ? 0m : document.TotalFinal
        }).ToList();
    }

    public async Task<ReceiptEmissionCatalog> CatalogAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var methods = await database.PaymentMethods.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => new ReceiptPayOption { Id = item.Id, Name = item.Designation, ExchangeRate = 1m })
            .ToListAsync(cancellationToken);
        var currencies = await database.Currencies.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => new ReceiptPayOption { Id = item.Id, Name = item.Designation, ExchangeRate = item.ExchangeRate })
            .ToListAsync(cancellationToken);
        return new ReceiptEmissionCatalog { PaymentMethods = methods, Currencies = currencies };
    }

    public async Task<ReceiptPayResult> PayAsync(ReceiptPayRequest request, CancellationToken cancellationToken = default)
    {
        var documentType = Portugal() ? "RC" : "RG";
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        if (await database.FiscalYears.AnyAsync(item => item.IsDeleted == false, cancellationToken) == false)
        {
            return ReceiptPayResult.Fail("Nenhum ano fiscal activo");
        }

        var seriesError = await MissingSeriesAsync(database, documentType, cancellationToken);
        if (seriesError is not null)
        {
            return ReceiptPayResult.Fail(seriesError);
        }

        var documents = await database.Documents
            .Where(item => request.DocumentIds.Contains(item.Id) && item.IsDeleted == false)
            .ToListAsync(cancellationToken);
        if (documents.Count != request.DocumentIds.Distinct().Count())
        {
            return ReceiptPayResult.Fail("Documento não encontrado.");
        }

        var net = documents.Where(item => item.Type is "FT" or "ND").Sum(item => item.TotalFinal)
            - documents.Where(item => item.Type == "NC").Sum(item => item.TotalFinal);
        if (request.Amount + 0.01m < ReceiptSettlement.Round(net))
        {
            return ReceiptPayResult.Fail("No modo local o recibo tem de liquidar o valor total em dívida.");
        }

        foreach (var document in documents.Where(item => item.Type is "FT" or "ND"))
        {
            document.Paid = true;
            document.UpdatedAt = DateTime.Now;
        }

        var currency = await database.Currencies.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CurrencyId, cancellationToken);
        database.Receipts.Add(new Receipt
        {
            PaymentMethodId = request.PaymentMethodId,
            DocumentType = documentType,
            RefNo = documentType + " " + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            Status = "N",
            SourcePayment = "P",
            Mechanism = "NU",
            Amount = request.Amount,
            AmountInWords = request.Amount.ToString("0.00", CultureInfo.CurrentCulture),
            SourceId = documents.First().Number,
            CurrencyCode = currency?.Acronym ?? "EUR",
            CurrencyAmount = request.CurrencyAmount,
            ExchangeRate = request.ExchangeRate,
            Notes = string.IsNullOrWhiteSpace(request.Notes)
                ? string.Join(",", documents.Select(item => item.Id))
                : request.Notes
        });
        await database.SaveChangesAsync(cancellationToken);
        return ReceiptPayResult.Done("Recibo emitido.");
    }

    private bool Portugal()
    {
        var country = _configuration["Country"];
        if (string.IsNullOrWhiteSpace(country) == false)
        {
            return string.Equals(country.Trim(), "PT", StringComparison.OrdinalIgnoreCase);
        }

        return CultureInfo.CurrentUICulture.Name.StartsWith("pt-PT", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string?> MissingSeriesAsync(LogicPOSDbContext database, string documentType, CancellationToken cancellationToken)
    {
        var type = await database.DocumentTypes.AsNoTracking()
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Acronym == documentType, cancellationToken);
        var hasSeries = type is not null && await database.DocumentSeries.AsNoTracking()
            .AnyAsync(series => series.IsDeleted == false && series.DocumentTypeId == type.Id, cancellationToken);
        if (hasSeries)
        {
            return null;
        }

        return $"Não existe uma série activa para o tipo de documento «{documentType}».{Environment.NewLine}{Environment.NewLine}Crie uma série deste tipo no BackOffice (Séries) antes de emitir.";
    }
}
