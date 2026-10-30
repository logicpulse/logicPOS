using System.Globalization;
using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Application.Features.System;
using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.Entities.Finance.Documents.Receipts.Settlement;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class LocalReceiptEmission : IReceiptEmission
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ISystemInformationService _systemInformation;

    public LocalReceiptEmission(IServiceScopeFactory scopes, ISystemInformationService systemInformation)
    {
        _scopes = scopes;
        _systemInformation = systemInformation;
    }

    public async Task<ReceiptEmissionPage> ListAsync(DateTime start, DateTime end, int page, int pageSize, string? search = null, CancellationToken cancellationToken = default, Guid? customerId = null)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var from = start.Date;
        var to = end.Date.AddDays(1);
        var query = database.Documents.AsNoTracking()
            .Include(document => document.Currency)
            .Include(document => document.Customer)
            .Where(document => document.IsDeleted == false
                && document.IsDraft == false
                && document.Paid == false
                && document.Status == "N"
                && (document.Type == "FT" || document.Type == "ND" || document.Type == "NC")
                && document.CreatedAt >= from
                && document.CreatedAt < to);
        if (customerId is Guid customer && customer != Guid.Empty)
        {
            query = query.Where(document => document.CustomerId == customer);
        }

        var size = Math.Clamp(pageSize, 1, 100);
        var current = Math.Max(1, page);
        var term = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        // Accent folding is not available in SQL Contains; filter after materialization when searching.
        List<Document> documents;
        int total;
        if (term is null)
        {
            total = await query.CountAsync(cancellationToken);
            documents = await query
                .OrderByDescending(document => document.CreatedAt)
                .Skip((current - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);
        }
        else
        {
            var matched = (await query
                .OrderByDescending(document => document.CreatedAt)
                .ToListAsync(cancellationToken))
                .Where(document => TextSearch.ContainsAny(
                    term,
                    document.Number,
                    document.Customer?.Name,
                    document.Customer?.FiscalNumber))
                .ToList();
            total = matched.Count;
            documents = matched
                .Skip((current - 1) * size)
                .Take(size)
                .ToList();
        }

        return new ReceiptEmissionPage
        {
            Page = current,
            TotalCount = total,
            TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size)),
            Items = documents.Select(document => new ReceiptEmissionRow
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
        }).ToList()
        };
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
            .Select(item => new ReceiptPayOption { Id = item.Id, Name = item.Designation, Code = item.Acronym, ExchangeRate = item.ExchangeRate })
            .ToListAsync(cancellationToken);
        return new ReceiptEmissionCatalog { PaymentMethods = methods, Currencies = currencies };
    }

    public async Task<ReceiptPayResult> PayAsync(ReceiptPayRequest request, CancellationToken cancellationToken = default)
    {
        var documentType = _systemInformation.IsPortugal() ? "RC" : "RG";
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

        var paidRows = await database.Payments.AsNoTracking()
            .Where(item => item.IsDeleted == false && request.DocumentIds.Contains(item.DocumentId))
            .GroupBy(item => item.DocumentId)
            .Select(group => new { group.Key, Amount = group.Sum(item => item.CreditAmount) })
            .ToListAsync(cancellationToken);
        var alreadyPaid = paidRows.ToDictionary(item => item.Key, item => item.Amount);
        decimal PaidOf(Document document) => alreadyPaid.TryGetValue(document.Id, out var amount) ? amount : 0m;

        var invoiceAmounts = documents.Where(item => item.Type is "FT" or "ND")
            .Select(item => ReceiptCredit.Outstanding(item.TotalFinal, PaidOf(item)));
        var creditAmounts = documents.Where(item => item.Type == "NC").Select(item => item.TotalFinal);
        var settleable = DocumentSettlementService.ComputeSettleableAmount(invoiceAmounts, creditAmounts);
        var receiptAmount = DocumentLineCalculator.ReceiptAmount(settleable, request.CurrencyAmount, request.ExchangeRate);
        if (request.Amount + 0.01m < settleable || receiptAmount + 0.01m < settleable)
        {
            return ReceiptPayResult.Fail("No modo local o recibo tem de liquidar o valor total em dívida.");
        }

        var country = _systemInformation.Country;
        var settlementDocs = documents
            .Select(item => new DocumentSettlementService.SettlementDocument(
                item.Id,
                item.Type,
                item.TotalFinal,
                item.TotalTax,
                item.WithholdingTaxAmount,
                PaidOf(item),
                item.CreatedAt))
            .ToList();
        var creditPool = creditAmounts.Sum(amount => amount.Round());
        var allocations = DocumentSettlementService.AllocateInvoicePayments(
            settlementDocs, receiptAmount, creditPool, country);
        var taxPayable = DocumentSettlementService.SumTaxPayable(allocations);
        var withholding = DocumentSettlementService.SumWithholding(allocations, country);

        var currency = await database.Currencies.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CurrencyId, cancellationToken);
        var receipt = new Receipt
        {
            PaymentMethodId = request.PaymentMethodId,
            DocumentType = documentType,
            RefNo = documentType + " " + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            Status = "N",
            SourcePayment = "P",
            Mechanism = "NU",
            Amount = receiptAmount,
            TaxPayable = taxPayable,
            WithholdingTaxAmount = withholding,
            AmountInWords = PortugueseMoneyWords.Convert(receiptAmount, currency?.Acronym ?? "EUR"),
            SourceId = documents.First().Number,
            CurrencyCode = currency?.Acronym ?? "EUR",
            CurrencyAmount = request.CurrencyAmount,
            ExchangeRate = request.ExchangeRate,
            Notes = string.IsNullOrWhiteSpace(request.Notes)
                ? string.Join(",", documents.Select(item => item.Id))
                : request.Notes
        };
        database.Receipts.Add(receipt);
        foreach (var allocation in allocations)
        {
            if (allocation.AmountToPay <= 0)
            {
                continue;
            }

            database.Payments.Add(new Payment
            {
                Receipt = receipt,
                DocumentId = allocation.DocumentId,
                CreditAmount = allocation.AmountToPay
            });
            var document = documents.First(item => item.Id == allocation.DocumentId);
            if (ReceiptCredit.Outstanding(document.TotalFinal, PaidOf(document) + allocation.AmountToPay) <= 0)
            {
                document.Paid = true;
                document.UpdatedAt = DateTime.Now;
            }
        }
        await database.SaveChangesAsync(cancellationToken);
        return ReceiptPayResult.Done("Recibo emitido.");
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
