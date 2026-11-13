using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using LogicPOS.Application.Features.System.DemoData;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DemoData;

/// <summary>
/// Inserts realistic FS sales documents for dashboard periods (today / 7d / 30d / month / year),
/// mirroring qtrack <c>DemoQueueDataService</c> safety: Demo licence + preference gate only.
/// </summary>
public sealed class DemoSalesDataService : IDemoSalesDataService
{
    private readonly LogicPOSDbContext _database;
    private readonly IDemoLicenseGate _licenseGate;
    private readonly ILogger<DemoSalesDataService> _logger;

    public DemoSalesDataService(
        LogicPOSDbContext database,
        IDemoLicenseGate licenseGate,
        ILogger<DemoSalesDataService> logger)
    {
        _database = database;
        _licenseGate = licenseGate;
        _logger = logger;
    }

    public async Task<DemoSalesDataRunResult> EnsureAsync(bool force = false, CancellationToken cancellationToken = default)
    {
        await DemoDataPreferenceSeeder.EnsureAsync(_database, cancellationToken).ConfigureAwait(false);

        if (_licenseGate.IsDemo == false)
        {
            return Skip("DemoDataNotLicensed");
        }

        var prefs = await _database.PreferenceParameters
            .Where(p => p.IsDeleted == false &&
                        (p.Token == DemoDataParameterTokens.Enabled
                         || p.Token == DemoDataParameterTokens.LastGeneratedDate))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var enabledPref = prefs.FirstOrDefault(p => p.Token == DemoDataParameterTokens.Enabled);
        if (IsTruthy(enabledPref?.Value) == false)
        {
            return Skip("DemoDataDisabled");
        }

        var today = DateTime.Today;
        var todayKey = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var lastPref = prefs.FirstOrDefault(p => p.Token == DemoDataParameterTokens.LastGeneratedDate);
        if (force == false && string.Equals(lastPref?.Value?.Trim(), todayKey, StringComparison.Ordinal))
        {
            return Skip("DemoDataAlreadyDone");
        }

        var catalog = await LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
        if (catalog is null)
        {
            return Skip("DemoDataNoCatalog");
        }

        var yearStart = new DateTime(today.Year, 1, 1);
        var fullWindow = force
                         || string.IsNullOrWhiteSpace(lastPref?.Value)
                         || !DateTime.TryParseExact(
                             lastPref!.Value.Trim(),
                             "yyyy-MM-dd",
                             CultureInfo.InvariantCulture,
                             DateTimeStyles.None,
                             out _);

        var from = fullWindow ? yearStart : today;
        var daysProcessed = 0;
        var documentsGenerated = 0;

        await ClearDemoDocumentsAsync(from, today.AddDays(1), cancellationToken).ConfigureAwait(false);

        for (var day = from; day <= today; day = day.AddDays(1))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var count = await GenerateDayAsync(day, catalog, cancellationToken).ConfigureAwait(false);
            if (count > 0)
            {
                daysProcessed++;
                documentsGenerated += count;
            }
        }

        await UpsertPreferenceAsync(
                DemoDataParameterTokens.LastGeneratedDate,
                todayKey,
                lastPref,
                cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Demo sales seed finished: days={Days}, documents={Docs}, force={Force}",
            daysProcessed, documentsGenerated, force);

        return new DemoSalesDataRunResult
        {
            Ran = true,
            DaysProcessed = daysProcessed,
            DocumentsGenerated = documentsGenerated,
            SummaryCode = force ? "DemoDataForced" : "DemoDataDone"
        };
    }

    private async Task<DemoCatalog?> LoadCatalogAsync(CancellationToken ct)
    {
        var series = await _database.DocumentSeries
            .AsNoTracking()
            .Include(s => s.DocumentType)
            .Where(s => s.IsDeleted == false
                        && s.DocumentType != null
                        && s.DocumentType.SaftDocumentType == SaftDocumentType.SalesInvoices
                        && (s.DocumentType.Acronym == "FS" || s.Acronym == "FS"))
            .OrderBy(s => s.Order)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (series is null)
        {
            series = await _database.DocumentSeries
                .AsNoTracking()
                .Include(s => s.DocumentType)
                .Where(s => s.IsDeleted == false
                            && s.DocumentType != null
                            && s.DocumentType.SaftDocumentType == SaftDocumentType.SalesInvoices)
                .OrderBy(s => s.Order)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        if (series?.DocumentType is null)
            return null;

        var customer = await _database.Customers
            .AsNoTracking()
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == CustomerWellKnownIds.FinalConsumer && c.IsDeleted == false, ct)
            .ConfigureAwait(false);
        if (customer is null)
            return null;

        var currencyId = await _database.Currencies
            .AsNoTracking()
            .Where(c => c.IsDeleted == false)
            .OrderBy(c => c.Order)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (currencyId == Guid.Empty)
            return null;

        var paymentMethodId = await _database.PaymentMethods
            .AsNoTracking()
            .Where(p => p.IsDeleted == false)
            .OrderBy(p => p.Order)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (paymentMethodId == Guid.Empty)
            return null;

        var terminalId = await _database.Terminals
            .AsNoTracking()
            .Where(t => t.IsDeleted == false)
            .OrderBy(t => t.Order)
            .Select(t => t.Id)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        var userId = await _database.Users
            .AsNoTracking()
            .Where(u => u.IsDeleted == false)
            .OrderBy(u => u.Order)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        var articles = await _database.Articles
            .AsNoTracking()
            .Include(a => a.VatDirectSelling)
            .Include(a => a.MeasurementUnit)
            .Where(a => a.IsDeleted == false && a.Price1 != null && a.Price1.Value > 0)
            .OrderBy(a => a.Order)
            .Take(40)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        if (articles.Count == 0)
            return null;

        return new DemoCatalog(
            series,
            customer,
            currencyId,
            paymentMethodId,
            terminalId == Guid.Empty ? Guid.Empty : terminalId,
            userId == Guid.Empty ? Guid.Empty : userId,
            articles);
    }

    private async Task ClearDemoDocumentsAsync(DateTime fromInclusive, DateTime toExclusive, CancellationToken ct)
    {
        var marker = DemoDataParameterTokens.DocumentNotesMarker;
        var docs = await _database.Documents
            .Where(d => d.Notes == marker && d.CreatedAt >= fromInclusive && d.CreatedAt < toExclusive)
            .Select(d => d.Id)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        if (docs.Count == 0)
            return;

        var payments = await _database.DocumentPaymentMethods
            .Where(p => docs.Contains(p.DocumentId))
            .ToListAsync(ct)
            .ConfigureAwait(false);
        _database.DocumentPaymentMethods.RemoveRange(payments);

        var details = await _database.DocumentDetails
            .Where(d => docs.Contains(d.DocumentId))
            .ToListAsync(ct)
            .ConfigureAwait(false);
        _database.DocumentDetails.RemoveRange(details);

        var masters = await _database.Documents
            .Where(d => docs.Contains(d.Id))
            .ToListAsync(ct)
            .ConfigureAwait(false);
        _database.Documents.RemoveRange(masters);

        await _database.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task<int> GenerateDayAsync(DateTime day, DemoCatalog catalog, CancellationToken ct)
    {
        var rng = new Random(HashCode.Combine(day.Year, day.Month, day.Day, 57005));
        var isWeekend = day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        var target = isWeekend
            ? rng.Next(2, 6)
            : rng.Next(6, 16);

        var docType = string.IsNullOrWhiteSpace(catalog.Series.DocumentType!.Acronym)
            ? catalog.Series.Acronym
            : catalog.Series.DocumentType.Acronym;
        if (string.IsNullOrWhiteSpace(docType))
            docType = "FS";

        var created = 0;
        for (var i = 0; i < target; i++)
        {
            var article = catalog.Articles[rng.Next(catalog.Articles.Count)];
            var vat = article.VatDirectSelling;
            if (vat is null)
                continue;

            var qty = rng.Next(1, 4);
            var unitPrice = article.Price1.Value;
            if (article.PriceWithVat && vat.Value > 0)
                unitPrice = Math.Round(unitPrice / (1 + vat.Value / 100m), 6);

            var totalNet = Math.Round(unitPrice * qty, 2);
            var totalTax = Math.Round(totalNet * (vat.Value / 100m), 2);
            var totalFinal = totalNet + totalTax;

            var hour = isWeekend ? rng.Next(10, 20) : SampleBusinessHour(rng);
            var minute = rng.Next(0, 60);
            var createdAt = day.AddHours(hour).AddMinutes(minute).AddSeconds(rng.Next(0, 60));

            var docId = DemoId($"{day:yyyyMMdd}-{i}");
            var detailId = DemoId($"{day:yyyyMMdd}-{i}-d");
            var payId = DemoId($"{day:yyyyMMdd}-{i}-p");

            var document = new Document
            {
                Id = docId,
                Notes = DemoDataParameterTokens.DocumentNotesMarker,
                Number = $"{catalog.Series.Acronym}/D{day:yyyyMMdd}{i + 1:D3}",
                Type = docType,
                Status = "N",
                StatusDate = createdAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                SourceBilling = "P",
                Hash = "0",
                HashControl = "0",
                IsDraft = false,
                SeriesId = catalog.Series.Id,
                CurrencyId = catalog.CurrencyId,
                CustomerId = catalog.Customer.Id,
                Customer = new DocumentCustomer
                {
                    Name = catalog.Customer.Name,
                    Address = catalog.Customer.Address,
                    Locality = catalog.Customer.Locality,
                    ZipCode = catalog.Customer.ZipCode,
                    City = catalog.Customer.City,
                    Country = catalog.Customer.Country?.Code2,
                    CountryId = catalog.Customer.CountryId,
                    FiscalNumber = string.IsNullOrWhiteSpace(catalog.Customer.FiscalNumber)
                        ? "999999990"
                        : catalog.Customer.FiscalNumber,
                    Email = catalog.Customer.Email,
                    Phone = catalog.Customer.Phone
                },
                ShipToAddress = new ShipAddress(),
                ShipFromAddress = new ShipAddress(),
                MovementStartTime = createdAt,
                MovementEndTime = createdAt,
                TotalNet = totalNet,
                TotalDiscount = 0,
                TotalTax = totalTax,
                TotalFinal = totalFinal,
                TotalDelivery = totalFinal,
                TotalChange = 0,
                Discount = 0,
                ExchangeRate = 1,
                Paid = true,
                PaymentDate = createdAt,
                Agt = new(),
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                CreatedBy = catalog.UserId,
                UpdatedBy = catalog.UserId,
                CreatedWhere = catalog.TerminalId,
                UpdatedWhere = catalog.TerminalId
            };

            var detail = new DocumentDetail
            {
                Id = detailId,
                DocumentId = docId,
                ArticleId = article.Id,
                Code = article.Code,
                Order = 1,
                Designation = article.Designation,
                Quantity = qty,
                Unit = article.MeasurementUnit?.Acronym,
                Price = unitPrice,
                Discount = 0,
                TotalNet = totalNet,
                TotalDiscount = 0,
                TotalTax = totalTax,
                TotalFinal = totalFinal,
                Tax = new DocumentDetailTax
                {
                    TaxId = vat.Id,
                    Designation = vat.Designation,
                    Type = vat.TaxType,
                    Code = vat.TaxCode,
                    Description = vat.Description,
                    Percentage = vat.Value,
                    CountryRegion = vat.CountryRegion
                },
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                CreatedBy = catalog.UserId,
                UpdatedBy = catalog.UserId,
                CreatedWhere = catalog.TerminalId,
                UpdatedWhere = catalog.TerminalId,
                Notes = DemoDataParameterTokens.DocumentNotesMarker
            };

            var payment = new DocumentPaymentMethod
            {
                Id = payId,
                DocumentId = docId,
                PaymentMethodId = catalog.PaymentMethodId,
                Amount = totalFinal,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                CreatedBy = catalog.UserId,
                UpdatedBy = catalog.UserId,
                CreatedWhere = catalog.TerminalId,
                UpdatedWhere = catalog.TerminalId,
                Notes = DemoDataParameterTokens.DocumentNotesMarker
            };

            _database.Documents.Add(document);
            _database.DocumentDetails.Add(detail);
            _database.DocumentPaymentMethods.Add(payment);
            created++;
        }

        if (created > 0)
            await _database.SaveChangesAsync(ct).ConfigureAwait(false);

        return created;
    }

    private async Task UpsertPreferenceAsync(
        string token,
        string value,
        PreferenceParameter? existing,
        CancellationToken ct)
    {
        if (existing is not null)
        {
            existing.Value = value;
            existing.UpdatedAt = DateTime.Now;
        }
        else
        {
            var row = await _database.PreferenceParameters
                .FirstOrDefaultAsync(p => p.Token == token && p.IsDeleted == false, ct)
                .ConfigureAwait(false);
            if (row is not null)
            {
                row.Value = value;
                row.UpdatedAt = DateTime.Now;
            }
        }

        await _database.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static int SampleBusinessHour(Random rng)
    {
        var roll = rng.NextDouble();
        if (roll < 0.35)
            return rng.Next(8, 12);
        if (roll < 0.55)
            return rng.Next(12, 15);
        return rng.Next(15, 20);
    }

    private static Guid DemoId(string key)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes("logicpos-demo-sales:" + key));
        return new Guid(hash);
    }

    private static bool IsTruthy(string? value) =>
        string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
        || value == "1"
        || string.Equals(value, "sim", StringComparison.OrdinalIgnoreCase);

    private static DemoSalesDataRunResult Skip(string code) => new()
    {
        Skipped = true,
        SummaryCode = code
    };

    private sealed record DemoCatalog(
        DocumentSeries Series,
        Customer Customer,
        Guid CurrencyId,
        Guid PaymentMethodId,
        Guid TerminalId,
        Guid UserId,
        IReadOnlyList<Article> Articles);
}
