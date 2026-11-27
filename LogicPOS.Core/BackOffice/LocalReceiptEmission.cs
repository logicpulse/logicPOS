using LogicPOS.Application.Features.Company;
using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Application.Features.System;
using LogicPOS.Core;
using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Services;
using LogicPOS.Core.FrontOffice;
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
        var from = start;
        var to = end < start ? start : end;
        // Document.Customer is a complex/owned type (not a navigation) — do not Include it.
        var query = database.Documents.AsNoTracking()
            .Include(document => document.Currency)
            .Where(document => document.IsDeleted == false
                && document.IsDraft == false
                && document.Paid == false
                && document.Status == "N"
                && (document.Type == "FT" || document.Type == "ND" || document.Type == "NC")
                && document.CreatedAt >= from
                && document.CreatedAt <= to);
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
            TotalFinal = DocumentMoneyTotal.Canonical(document.TotalFinal),
            TotalPaid = 0m,
            TotalToPay = document.Type == "NC" ? 0m : DocumentMoneyTotal.Canonical(document.TotalFinal)
        }).ToList()
        };
    }

    public async Task<ReceiptEmissionCatalog> CatalogAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var methods = await database.PaymentMethods.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Order)
            .ThenBy(item => item.Designation)
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
            return ReceiptPayResult.Fail("Nenhum ano fiscal ativo");
        }

        var seriesError = await MissingSeriesAsync(database, documentType, cancellationToken);
        if (seriesError is not null)
        {
            return ReceiptPayResult.Fail(seriesError);
        }

        var fiscal = scope.ServiceProvider.GetService<IFiscalModule>();
        var receiptSeries = await scope.ServiceProvider.GetRequiredService<IDocumentSeriesRepository>()
            .GetActiveSeriesForReceiptCreationAsync(null, cancellationToken);
        var atRefusal = FiscalSeriesAtRules.RefusalMessageIfMissing(
            documentType,
            receiptSeries?.ATDocCodeValidationSeries,
            _systemInformation.Country,
            fiscal?.IsAvailable == true);
        if (atRefusal is not null)
        {
            return ReceiptPayResult.Fail(atRefusal);
        }

        var documents = await database.Documents
            .Include(item => item.Currency)
            .Include(item => item.Details)
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

        var series = await database.DocumentSeries
            .Include(item => item.DocumentType)
            .Include(item => item.FiscalYear)
            .Where(item => item.IsDeleted == false
                && item.FiscalYear!.IsDeleted == false
                && item.DocumentType!.SaftDocumentType == SaftDocumentType.Payments)
            .FirstOrDefaultAsync(cancellationToken);
        if (series is null)
        {
            return ReceiptPayResult.Fail("Nenhuma série ativa para emissão de recibos foi encontrada.");
        }

        var refNo = ReceiptEmissionDefaults.TakeNextReceiptNumber(series);
        var paymentMethod = await database.PaymentMethods.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.PaymentMethodId, cancellationToken);

        var currency = await database.Currencies.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CurrencyId, cancellationToken);
        var receipt = new Receipt
        {
            PaymentMethodId = request.PaymentMethodId,
            DocumentType = documentType,
            RefNo = refNo,
            Status = "N",
            SourcePayment = "P",
            Mechanism = paymentMethod?.Acronym ?? "NU",
            Amount = receiptAmount,
            TaxPayable = taxPayable,
            WithholdingTaxAmount = withholding,
            AmountInWords = PortugueseMoneyWords.Convert(receiptAmount, currency?.Acronym ?? "EUR"),
            SourceId = documents.First().Number,
            CurrencyCode = currency?.Acronym ?? "EUR",
            CurrencyAmount = request.CurrencyAmount,
            ExchangeRate = request.ExchangeRate,
            // User notes only — document links live on Payments / RelatedDocs, not as GUIDs in Notes.
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? string.Empty : request.Notes.Trim()
        };
        if (_systemInformation.IsPortugal()
            && string.IsNullOrWhiteSpace(series.ATDocCodeValidationSeries) == false)
        {
            receipt.Atcud = $"{series.ATDocCodeValidationSeries}-{series.NextNumber - 1}";
        }

        if (fiscal?.IsAvailable == true && _systemInformation.IsPortugal())
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IDocumentHasher>();
            var lastReceiptNumber = $"{series.Acronym}/{series.NextNumber - 1}";
            receipt.Hash = await hasher.GenerateReceiptHashAsync(
                lastReceiptNumber,
                refNo,
                receipt.Date,
                receipt.SystemEntryDate,
                receipt.Amount,
                cancellationToken);
            var hash4 = hasher.GenerateHash4Chars(receipt.Hash);
            if (string.IsNullOrWhiteSpace(hash4))
            {
                hash4 = DocumentCertificationText.ExtractHash4Chars(receipt.Hash);
            }

            string? issuerFiscalNumber = null;
            var companyInformation = scope.ServiceProvider.GetService<ICompanyInformationService>();
            if (companyInformation is not null)
            {
                var company = await companyInformation.GetCompanyInformationAsync(cancellationToken);
                issuerFiscalNumber = company.FiscalNumber;
            }
            else
            {
                issuerFiscalNumber = await database.PreferenceParameters.AsNoTracking()
                    .Where(item => item.Token == "COMPANY_FISCALNUMBER")
                    .Select(item => item.Value)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var customer = documents[0].Customer;
            var settledDocuments = BuildAtQrSettledDocuments(allocations, documents);
            var settlement = new AtQrReceiptSettlement(
                receipt.Amount,
                receipt.TaxPayable,
                receipt.WithholdingTaxAmount,
                receipt.CurrencyCode,
                receipt.ExchangeRate,
                settledDocuments);
            var breakdown = AtQrReceiptBreakdown.FromSettledDocuments(
                settlement,
                ReceiptEmissionDefaults.ReceiptAtQrPartialPaymentAllocation);
            receipt.QrCode = DocumentAtQrPayload.Build(new AtQrPayloadInput(
                PosDocumentRules.AtQrIssuerFiscalNumber(issuerFiscalNumber),
                PosDocumentRules.AtQrCustomerFiscalNumber(customer.FiscalNumber),
                customer.Country ?? "PT",
                documentType,
                receipt.Status,
                receipt.Date,
                refNo,
                receipt.Atcud,
                breakdown.TaxBreakdown,
                breakdown.TotalTax,
                breakdown.TotalFinal,
                hash4,
                WithholdingTaxAmount: breakdown.WithholdingTaxAmount,
                CurrencyAcronym: breakdown.CurrencyAcronym,
                ExchangeRate: breakdown.ExchangeRate));
        }

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

    private static IReadOnlyList<AtQrSettledDocument> BuildAtQrSettledDocuments(
        IReadOnlyList<DocumentSettlementService.InvoicePaymentAllocation> allocations,
        IReadOnlyList<Document> documents)
    {
        var byId = documents.ToDictionary(item => item.Id);
        var settled = new List<AtQrSettledDocument>();
        foreach (var allocation in allocations)
        {
            if (allocation.AmountToPay <= 0)
            {
                continue;
            }

            if (byId.TryGetValue(allocation.DocumentId, out var document) == false)
            {
                continue;
            }

            // NC credits are applied via the settlement pool only; they are not AT QR settled lines.
            if (string.Equals(document.Type, "NC", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var lines = (document.Details ?? Array.Empty<DocumentDetail>())
                .Select(detail => new AtQrSettledDocumentLine(
                    detail.Tax.CountryRegion,
                    detail.Tax.Code,
                    detail.TotalNet,
                    detail.TotalTax))
                .ToList();

            var currencyCode = document.Currency?.Acronym;
            var exchangeRate = document.ExchangeRate > 0m ? document.ExchangeRate : 1m;
            settled.Add(new AtQrSettledDocument(
                allocation.AmountToPay,
                document.TotalFinal,
                document.TotalTax,
                document.WithholdingTaxAmount,
                lines,
                currencyCode,
                exchangeRate));
        }

        return settled;
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

        return $"Não existe uma série ativa para o tipo de documento «{documentType}».{Environment.NewLine}{Environment.NewLine}Crie uma série deste tipo no BackOffice (Séries) antes de emitir.";
    }
}
