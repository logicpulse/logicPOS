using System.Reflection;
using System.Net;
using System.Net.Mail;
using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Documents;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Services;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using LogicPOS.Shared.Features.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PdfDocument = LogicPOS.Application.Features.Finance.Documents.PdfGeneration.Document;
using PdfDocumentDetail = LogicPOS.Application.Features.Finance.Documents.PdfGeneration.DocumentDetail;
using PdfPaymentMethod = LogicPOS.Application.Features.Finance.Documents.PdfGeneration.PaymentMethod;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosSaleLine
{
    public PosSaleLine(Guid articleId, decimal quantity, decimal netUnitPrice, decimal discount, Guid vatRateId, string? notes = null, string? serialNumber = null)
    {
        ArticleId = articleId;
        Quantity = quantity;
        NetUnitPrice = netUnitPrice;
        Discount = discount;
        VatRateId = vatRateId;
        Notes = notes;
        SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber.Trim();
    }

    public Guid ArticleId { get; }

    public decimal Quantity { get; }

    public decimal NetUnitPrice { get; }

    public decimal Discount { get; }

    public Guid VatRateId { get; }

    public string? Notes { get; }

    public string? SerialNumber { get; }
}

public sealed class PosLookupItem
{
    public PosLookupItem(Guid id, string label)
        : this(id, label, string.Empty)
    {
    }

    public PosLookupItem(Guid id, string label, string? code)
    {
        Id = id;
        Label = label;
        Code = code ?? string.Empty;
    }

    public Guid Id { get; }

    public string Label { get; }

    public string Code { get; }

    public override string ToString() => Label;
}

public sealed class PosVatOption
{
    public PosVatOption(Guid id, string label, decimal percentage)
    {
        Id = id;
        Label = label;
        Percentage = percentage;
    }

    public Guid Id { get; }

    public string Label { get; }

    public decimal Percentage { get; }

    public override string ToString() => Label;
}

public sealed class PosDocumentHeader
{
    public Guid? PaymentConditionId { get; init; }

    public Guid? CurrencyId { get; init; }

    public Guid? ParentDocumentId { get; init; }

    public string? PaymentToken { get; init; }

    public string? Notes { get; init; }

    public bool IsDraft { get; init; }

    public PosShipAddress? ShipTo { get; init; }

    public PosShipAddress? ShipFrom { get; init; }
}

public sealed class PosShipAddress
{
    public string? Address { get; init; }

    public string? Region { get; init; }

    public string? PostalCode { get; init; }

    public string? City { get; init; }

    public string? Country { get; init; }

    public string? When { get; init; }

    public string? DeliveryId { get; init; }

    public string? WarehouseId { get; init; }

    public string? LocationId { get; init; }
}

public sealed class PosCompanyAddress
{
    public string Address { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string Country { get; init; } = "Portugal";
}

public static class PosDocumentRules
{
    public const string Unknown = "Desconhecido";

    public static bool IsWayBill(string? acronym)
    {
        return string.IsNullOrWhiteSpace(acronym) == false && new DocumentTypeAnalyzer(acronym).IsWayBill();
    }

    public static bool RequiresNamedCustomer(string? acronym)
    {
        return acronym is "FT" or "NC" or "ND";
    }

    public static bool SupportsOptionalTransport(string? acronym)
    {
        return string.IsNullOrWhiteSpace(acronym) == false && new DocumentTypeAnalyzer(acronym).IsSalesInvoiceFamily();
    }

    public static string? ValidateTransport(PosShipAddress destination, PosShipAddress origin)
    {
        var shipTo = ToAddress(destination);
        var shipFrom = ToAddress(origin);
        if (shipTo.HasRequiredTransportFields(true) == false || IsPlaceholder(destination))
        {
            return "Preencha a morada de destino. Em Portugal o código postal é obrigatório.";
        }

        if (shipFrom.HasRequiredTransportFields(true) == false || IsPlaceholder(origin))
        {
            return "Preencha a morada de origem. Em Portugal o código postal é obrigatório.";
        }

        if (IsPostalCode(destination.PostalCode) == false || IsPostalCode(origin.PostalCode) == false)
        {
            return "O código postal tem de ter o formato 0000-000.";
        }

        var dates = CheckTransportDates(ParseWhen(origin.When), ParseWhen(destination.When));
        if (dates.Message is not null)
        {
            return dates.Message;
        }

        if (ShipAddress.HasCompleteTransportData(shipTo, shipFrom, true) == false)
        {
            return "A data/hora de entrega tem de ser posterior à data/hora de expedição.";
        }

        return null;
    }

    public static (bool DispatchValid, bool ArrivalValid, string? Message) CheckTransportDates(DateTime? dispatch, DateTime? arrival)
    {
        var today = DateTime.Today;
        var dispatchValid = dispatch.HasValue && dispatch.Value.Date >= today;
        var arrivalOnOrAfterToday = arrival.HasValue && arrival.Value.Date >= today;
        // Compare full date/time — same calendar day with a later delivery time is valid.
        var arrivalAfterDispatch = dispatch.HasValue && arrival.HasValue && arrival.Value > dispatch.Value;
        var arrivalValid = arrivalOnOrAfterToday && arrivalAfterDispatch;

        string? message = null;
        if (dispatchValid == false && arrivalOnOrAfterToday == false)
        {
            message = "As datas de expedição e de entrega não podem ser anteriores a hoje.";
        }
        else if (dispatchValid == false)
        {
            message = "A data de expedição não pode ser anterior a hoje.";
        }
        else if (arrivalOnOrAfterToday == false)
        {
            message = "A data de entrega não pode ser anterior a hoje.";
        }
        else if (dispatch.HasValue && dispatch.Value <= DateTime.Now)
        {
            dispatchValid = false;
            message = "A hora de expedição já passou. Corrija a hora antes de emitir.";
        }
        else if (arrivalAfterDispatch == false)
        {
            message = "A data/hora de entrega tem de ser posterior à data/hora de expedição.";
        }

        return (dispatchValid, arrivalValid, message);
    }

    public static ShipAddress ToAddress(PosShipAddress address)
    {
        return new ShipAddress
        {
            AddressDetail = address.Address?.Trim(),
            Region = address.Region?.Trim(),
            PostalCode = address.PostalCode?.Trim(),
            City = address.City?.Trim(),
            Country = CountryCode(address.Country),
            DeliveryDate = ParseWhen(address.When),
            DeliveryID = address.DeliveryId?.Trim(),
            WarehouseID = address.WarehouseId?.Trim(),
            LocationID = address.LocationId?.Trim()
        };
    }

    private static bool IsPlaceholder(PosShipAddress address)
    {
        return IsUnknown(address.Address)
            || IsUnknown(address.Region)
            || IsUnknown(address.City)
            || IsUnknown(address.Country)
            || IsPlaceholderPostal(address.PostalCode);
    }

    private static bool IsUnknown(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || string.Equals(value.Trim(), Unknown, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPlaceholderPostal(string? value)
    {
        var text = value?.Trim();
        return string.IsNullOrWhiteSpace(text) || text is "0000-000" or "000-000";
    }

    private static bool IsPostalCode(string? value)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(value?.Trim() ?? string.Empty, @"^\d{4}-\d{3}$");
    }

    private static string CountryCode(string? value)
    {
        var text = value?.Trim() ?? string.Empty;
        if (text.Length == 0 || text.Equals("Portugal", StringComparison.OrdinalIgnoreCase) || text.Equals("PT", StringComparison.OrdinalIgnoreCase))
        {
            return "PT";
        }

        return text;
    }

    private static DateTime? ParseWhen(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var text = value.Trim();
        if (DateTime.TryParseExact(text, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var exact))
        {
            return exact;
        }

        return DateTime.TryParse(text, System.Globalization.CultureInfo.GetCultureInfo("pt-PT"), System.Globalization.DateTimeStyles.None, out var parsed)
            ? parsed
            : null;
    }
}

public sealed class PosCopiedLine
{
    public PosCopiedLine(
        Guid articleId,
        string code,
        string designation,
        decimal quantity,
        decimal price,
        decimal discount,
        Guid vatRateId,
        decimal vatPercentage,
        string? serialNumber = null)
    {
        ArticleId = articleId;
        Code = code;
        Designation = designation;
        Quantity = quantity;
        Price = price;
        Discount = discount;
        VatRateId = vatRateId;
        VatPercentage = vatPercentage;
        SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber.Trim();
    }

    public Guid ArticleId { get; }

    public string Code { get; }

    public string Designation { get; }

    public decimal Quantity { get; }

    public decimal Price { get; }

    public decimal Discount { get; }

    public Guid VatRateId { get; }

    public decimal VatPercentage { get; }

    public string? SerialNumber { get; }
}

public sealed class PosDocumentResult
{
    private PosDocumentResult(bool succeeded, Guid documentId, string? number, string? error)
    {
        Succeeded = succeeded;
        DocumentId = documentId;
        Number = number;
        Error = error;
    }

    public bool Succeeded { get; }

    public Guid DocumentId { get; }

    public string? Number { get; }

    public string? Error { get; }

    public static PosDocumentResult Ok(string number) => new(true, Guid.Empty, number, null);

    public static PosDocumentResult Ok(Guid documentId, string number) => new(true, documentId, number, null);

    public static PosDocumentResult Fail(string error) => new(false, Guid.Empty, null, error);
}

public sealed class PosDocumentService : IPosDocumentService
{
    private const string SimplifiedInvoice = "FS";

    private readonly IServiceScopeFactory _scopes;

    public PosDocumentService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<PosDocumentResult> IssueSimplifiedInvoiceAsync(
        Guid customerId,
        string paymentToken,
        decimal discountPercent,
        decimal? amountDelivered,
        string? notes,
        IReadOnlyList<PosSaleLine> lines,
        string documentType = "FS",
        CancellationToken cancellationToken = default)
    {
        if (lines.Count == 0)
        {
            return PosDocumentResult.Fail("O ticket não tem artigos.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var paymentMethod = await database.PaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(method => method.IsDeleted == false && method.Token == paymentToken, cancellationToken);

        if (paymentMethod is null)
        {
            return PosDocumentResult.Fail("Método de pagamento não encontrado.");
        }

        var seriesResult = await EnsureSeriesAsync(scope.ServiceProvider, database, documentType, cancellationToken);
        if (seriesResult.Succeeded == false)
        {
            return seriesResult;
        }

        var details = lines.Select(line => new CreateDocumentDetailDto(
            ArticleId: line.ArticleId,
            Quantity: line.Quantity,
            Price: line.NetUnitPrice,
            VatExemptionId: null,
            VatRateId: line.VatRateId == Guid.Empty ? null : line.VatRateId,
            Discount: line.Discount,
            PriceType: null,
            Notes: null,
            Country2Code: "PT",
            SerialNumber: null)).ToList();

        var dto = new CreateDocumentDto(
            Type: documentType,
            CustomerId: customerId,
            PaymentConditionId: null,
            CurrencyId: null,
            ParentId: null,
            ShipToAddress: null,
            ShipFromAddress: null,
            Discount: discountPercent,
            Customer: null,
            ExchangeRate: 1,
            TotalDelivery: amountDelivered,
            Notes: string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Details: details,
            PaymentMethods: new[] { new CreateDocumentPaymentMethodDto(paymentMethod.Id, 0) },
            IsDraft: false);

        var references = new DocumentReferences(
            scope.ServiceProvider.GetRequiredService<IDocumentRepository>(),
            scope.ServiceProvider.GetRequiredService<IPaymentMethodRepository>(),
            scope.ServiceProvider.GetRequiredService<IDocumentSeriesRepository>(),
            scope.ServiceProvider.GetRequiredService<ICurrencyRepository>(),
            scope.ServiceProvider.GetRequiredService<ICustomerRepository>(),
            scope.ServiceProvider.GetRequiredService<IPaymentConditionRepository>(),
            scope.ServiceProvider.GetRequiredService<IArticleRepository>(),
            scope.ServiceProvider.GetRequiredService<IVatRateRepository>(),
            scope.ServiceProvider.GetRequiredService<IVatExemptionReasonRepository>());

        var created = await Document.CreateAsync(
            dto,
            references.DocumentRepository,
            references,
            scope.ServiceProvider.GetRequiredService<IDocumentHasher>(),
            FiscalCountry.Portugal,
            terminalId: null,
            cancellationToken);

        if (created.IsFailure)
        {
            return PosDocumentResult.Fail(Explain(created.Error.Message));
        }

        var document = created.Value!;
        await database.Documents.AddAsync(document, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(document.Number);
    }

    private static async Task<PosDocumentResult> EnsureSeriesAsync(
        IServiceProvider services,
        LogicPOSDbContext database,
        string documentType,
        CancellationToken cancellationToken)
    {
        var existing = await services.GetRequiredService<IDocumentSeriesRepository>()
            .GetActiveSeriesByDocumentTypeForDocumentCreationAsync(documentType, null, cancellationToken);
        if (existing is not null)
        {
            return PosDocumentResult.Ok(existing.Acronym);
        }

        var year = DateTime.Now.Year;
        var fiscalYear = await database.FiscalYears
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Year == year, cancellationToken);

        if (fiscalYear is null)
        {
            var createdYear = await FiscalYear.CreateAsync(
                new CreateFiscalYearDto(year.ToString(), year.ToString(), year, false, null),
                services.GetRequiredService<IFiscalYearRepository>(),
                cancellationToken);
            if (createdYear.IsFailure)
            {
                return PosDocumentResult.Fail(createdYear.Error.Message);
            }

            fiscalYear = createdYear.Value!;
            await database.FiscalYears.AddAsync(fiscalYear, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
        }

        var documentTypeEntity = await database.DocumentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Acronym == documentType, cancellationToken);
        if (documentTypeEntity is null)
        {
            return PosDocumentResult.Fail($"Não existe o tipo de documento {documentType}.");
        }

        var createdSeries = await DocumentSeries.CreateAsync(
            new CreateDocumentSeriesDto(
                NextNumber: 1,
                NumberRangeBegin: 1,
                NumberRangeEnd: 9999999,
                Designation: $"{documentTypeEntity.Designation} {year}",
                Acronym: $"{documentType}{year}",
                DocumentTypeId: documentTypeEntity.Id,
                FiscalYearId: fiscalYear.Id,
                AtValidationCode: null,
                Notes: null),
            services.GetRequiredService<IDocumentSeriesRepository>(),
            new DocumentSeriesReferences(
                services.GetRequiredService<IFiscalYearRepository>(),
                services.GetRequiredService<IDocumentTypeRepository>()),
            cancellationToken);

        if (createdSeries.IsFailure)
        {
            return PosDocumentResult.Fail(createdSeries.Error.Message);
        }

        await database.DocumentSeries.AddAsync(createdSeries.Value!, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(createdSeries.Value!.Acronym);
    }

    public async Task EnsureFiscalSetupAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var database = services.GetRequiredService<LogicPOSDbContext>();
        var year = DateTime.Now.Year;
        var fiscalYear = await database.FiscalYears
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Year == year, cancellationToken);

        if (fiscalYear is null)
        {
            var createdYear = await FiscalYear.CreateAsync(
                new CreateFiscalYearDto(year.ToString(), year.ToString(), year, false, null),
                services.GetRequiredService<IFiscalYearRepository>(),
                cancellationToken);
            if (createdYear.IsFailure)
            {
                return;
            }

            fiscalYear = createdYear.Value!;
            await database.FiscalYears.AddAsync(fiscalYear, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
        }

        var documentTypes = await database.DocumentTypes
            .Where(item => item.IsDeleted == false && item.Acronym != "")
            .ToListAsync(cancellationToken);
        var series = await database.DocumentSeries
            .Where(item => item.IsDeleted == false)
            .ToListAsync(cancellationToken);

        foreach (var documentType in documentTypes)
        {
            var current = series.FirstOrDefault(item => item.DocumentTypeId == documentType.Id);
            if (current is not null)
            {
                continue;
            }

            var designation = string.IsNullOrWhiteSpace(documentType.Designation)
                ? documentType.Acronym
                : documentType.Designation;
            var createdSeries = await DocumentSeries.CreateAsync(
                new CreateDocumentSeriesDto(
                    NextNumber: 1,
                    NumberRangeBegin: 1,
                    NumberRangeEnd: 9999999,
                    Designation: $"{designation} {year}",
                    Acronym: $"{documentType.Acronym}{year}",
                    DocumentTypeId: documentType.Id,
                    FiscalYearId: fiscalYear.Id,
                    AtValidationCode: null,
                    Notes: null),
                services.GetRequiredService<IDocumentSeriesRepository>(),
                new DocumentSeriesReferences(
                    services.GetRequiredService<IFiscalYearRepository>(),
                    services.GetRequiredService<IDocumentTypeRepository>()),
                cancellationToken);
            if (createdSeries.IsSuccess)
            {
                await database.DocumentSeries.AddAsync(createdSeries.Value!, cancellationToken);
            }
        }

        await database.SaveChangesAsync(cancellationToken);
    }

    private static string Explain(string message)
    {
        if (message.Contains("não foi comunicada", StringComparison.OrdinalIgnoreCase))
        {
            return "Não foi possível gravar a fatura simplificada porque a série ainda não foi comunicada à AT. O ticket mantém-se.";
        }

        return message;
    }

    public async Task<IReadOnlyList<string>> ListRecentAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var documents = await database.Documents
            .AsNoTracking()
            .Where(document => document.IsDeleted == false)
            .OrderByDescending(document => document.CreatedAt)
            .Take(40)
            .Select(document => new { document.Number, document.CreatedAt, document.TotalFinal })
            .ToListAsync(cancellationToken);

        return documents
            .Select(document => $"{document.Number}    {document.CreatedAt:dd/MM/yyyy HH:mm}    {document.TotalFinal:C}")
            .ToList();
    }

    public async Task<(decimal Day, decimal Month, decimal Year)> GetSalesTotalsAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var now = DateTime.Now;
        var startYear = new DateTime(now.Year, 1, 1);
        var startMonth = new DateTime(now.Year, now.Month, 1);
        var startDay = now.Date;
        var documents = await database.Documents
            .AsNoTracking()
            .Where(document => document.IsDeleted == false && document.CreatedAt >= startYear && document.CreatedAt < startDay.AddDays(1))
            .Select(document => new { document.CreatedAt, document.TotalFinal })
            .ToListAsync(cancellationToken);

        return (
            documents.Where(document => document.CreatedAt >= startDay).Sum(document => document.TotalFinal),
            documents.Where(document => document.CreatedAt >= startMonth).Sum(document => document.TotalFinal),
            documents.Sum(document => document.TotalFinal));
    }

    public async Task<PosDocumentPage> ListByPeriodAsync(
        DateTime start,
        DateTime end,
        int page,
        int pageSize,
        string? search = null,
        CancellationToken cancellationToken = default,
        Guid? customerId = null,
        string? documentType = null)
    {
        var from = start.Date;
        var to = end.Date.AddDays(1);
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var query = database.Documents
            .AsNoTracking()
            .Where(document => document.IsDeleted == false && document.CreatedAt >= from && document.CreatedAt < to);
        if (customerId is Guid customer && customer != Guid.Empty)
        {
            query = query.Where(document => document.CustomerId == customer);
        }

        if (string.IsNullOrWhiteSpace(documentType) == false)
        {
            var type = documentType.Trim();
            query = query.Where(document => document.Type == type);
        }

        if (string.IsNullOrWhiteSpace(search) == false)
        {
            var term = search.Trim();
            query = query.Where(document =>
                document.Number.Contains(term)
                || document.Customer.Name.Contains(term)
                || document.Customer.FiscalNumber.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var size = Math.Clamp(pageSize, 1, 100);
        var current = Math.Max(1, page);
        var documents = await query
            .OrderByDescending(document => document.CreatedAt)
            .Skip((current - 1) * size)
            .Take(size)
            .Select(document => new
            {
                document.Id,
                document.CreatedAt,
                document.Number,
                document.Status,
                document.IsDraft,
                document.TotalFinal,
                document.Paid,
                document.ParentId,
                EntityName = document.Customer.Name,
                TaxId = document.Customer.FiscalNumber
            })
            .ToListAsync(cancellationToken);

        var parentIds = documents.Where(document => document.ParentId != null).Select(document => document.ParentId!.Value).Distinct().ToList();
        var parentNumbers = parentIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await database.Documents
                .AsNoTracking()
                .Where(document => parentIds.Contains(document.Id))
                .Select(document => new { document.Id, document.Number })
                .ToDictionaryAsync(document => document.Id, document => document.Number, cancellationToken);

        var rows = documents.Select(document =>
        {
            var paid = document.Paid ? document.TotalFinal : 0m;
            var associated = document.ParentId is Guid parentId && parentNumbers.TryGetValue(parentId, out var number)
                ? number
                : string.Empty;
            return new PosDocumentRow
            {
                Id = document.Id,
                DocumentDate = document.CreatedAt,
                Number = document.Number,
                Status = DescribeStatus(document.IsDraft, document.Status),
                EntityName = document.EntityName,
                TaxId = document.TaxId,
                FinalTotal = document.TotalFinal,
                PaidAmount = paid,
                DebitAmount = document.TotalFinal - paid,
                AssociatedDocuments = associated
            };
        }).ToList();
        if (current <= 1 && string.IsNullOrWhiteSpace(search) && customerId is null && string.IsNullOrWhiteSpace(documentType))
        {
            rows = LocalDraftStore.Merge(rows, start, end).ToList();
        }

        return new PosDocumentPage
        {
            Items = rows,
            Page = current,
            TotalCount = total,
            TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size))
        };
    }

    private static string DescribeStatus(bool isDraft, string status)
    {
        if (isDraft)
        {
            return "Rascunho";
        }

        return status switch
        {
            "N" => "Normal",
            "A" => "Anulado",
            "F" => "Faturado",
            _ => status
        };
    }

    public async Task<IReadOnlyList<PosDocumentTypeOption>> ListDocumentTypesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.DocumentTypes
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Acronym != "")
            .OrderBy(item => item.Designation)
            .Select(item => new PosDocumentTypeOption(item.Id, item.Acronym, item.Designation))
            .ToListAsync(cancellationToken);
    }

    public async Task<string> GetSeriesLabelAsync(string documentType, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var series = await scope.ServiceProvider.GetRequiredService<IDocumentSeriesRepository>()
            .GetActiveSeriesByDocumentTypeForDocumentCreationAsync(documentType, null, cancellationToken);
        return series?.Acronym ?? string.Empty;
    }

    public async Task<IReadOnlyList<PosLookupItem>> ListPaymentConditionsAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.PaymentConditions
            .AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Order)
            .ThenBy(item => item.Designation)
            .Select(item => new PosLookupItem(item.Id, item.Designation, item.Acronym))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PosLookupItem>> ListCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.Currencies
            .AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Order)
            .ThenBy(item => item.Designation)
            .Select(item => new PosLookupItem(item.Id, item.Designation, item.Acronym))
            .ToListAsync(cancellationToken);
    }

    public async Task<PosCompanyAddress> GetCompanyAddressAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var company = await ReadCompanyAsync(database, cancellationToken);
        return new PosCompanyAddress
        {
            Address = company.Address ?? string.Empty,
            City = company.City ?? string.Empty,
            PostalCode = company.PostalCode ?? string.Empty,
            Country = string.Equals(company.CountryCode2, "PT", StringComparison.OrdinalIgnoreCase) ? "Portugal" : company.CountryCode2 ?? "Portugal"
        };
    }

    public async Task DeleteDraftAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        if (LocalDraftStore.Delete(documentId))
        {
            return;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var document = await database.Documents
            .FirstOrDefaultAsync(item => item.Id == documentId && item.IsDraft && item.IsDeleted == false, cancellationToken);
        if (document is null)
        {
            return;
        }

        document.IsDeleted = true;
        document.DeletedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
    }

    public async Task<string?> CancelDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var document = await database.Documents.FirstOrDefaultAsync(item => item.Id == documentId && item.IsDeleted == false, cancellationToken);
        if (document is null)
        {
            return "Documento não encontrado.";
        }

        if (document.IsDraft)
        {
            document.IsDeleted = true;
            document.DeletedAt = DateTime.Now;
            await database.SaveChangesAsync(cancellationToken);
            return null;
        }

        document.Status = "A";
        document.StatusReason = string.IsNullOrWhiteSpace(reason) ? "Anulação" : reason.Trim();
        document.StatusDate = DateTime.Now.ToString("yyyy-MM-dd");
        await database.SaveChangesAsync(cancellationToken);
        var fiscal = scope.ServiceProvider.GetService<IFiscalModule>();
        if (fiscal is { IsAvailable: true })
        {
            var note = await fiscal.NotifyCancellationAsync(documentId, document.StatusReason, cancellationToken);
            if (string.IsNullOrWhiteSpace(note) == false)
            {
                return "Documento anulado. " + note;
            }
        }

        return document.Type is "FT" or "FS" or "FR"
            ? "Documento anulado. Emita uma nota de crédito se a certificação o exigir."
            : null;
    }

    public async Task<PosEmailDraft> PrepareDocumentEmailAsync(IReadOnlyList<string> documentNumbers, string? fiscalNumber, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var parameters = await database.PreferenceParameters
            .AsNoTracking()
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);
        string? Value(string token) => parameters.FirstOrDefault(item => item.Token == token)?.Value;
        var company = await ReadCompanyAsync(database, cancellationToken);
        string? to = null;
        if (string.IsNullOrWhiteSpace(fiscalNumber) == false)
        {
            to = await database.Customers.AsNoTracking()
                .Where(item => item.IsDeleted == false && item.FiscalNumber == fiscalNumber)
                .Select(item => item.Email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var html = string.Equals(Value("SEND_MAIL_FINANCE_DOCUMENTS_HTML_BODY"), "true", StringComparison.OrdinalIgnoreCase);
        return FinanceEmailTemplate.Compose(
            Value("SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT"),
            Value("SEND_MAIL_FINANCE_DOCUMENTS_BODY"),
            html,
            company.Name,
            company.BusinessName,
            company.Website,
            company.Email,
            company.Phone,
            company.Address,
            company.PostalCode,
            company.City,
            company.CountryCode2,
            documentNumbers,
            to);
    }

    public async Task<string?> SendDocumentsByEmailAsync(
        IReadOnlyList<Guid> documentIds,
        string to,
        string subject,
        string body,
        string? cc = null,
        string? bcc = null,
        bool sendReceipts = false,
        CancellationToken cancellationToken = default)
    {
        _ = sendReceipts;
        await using var scope = _scopes.CreateAsyncScope();
        var configuration = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        var host = configuration["LogicPOS:Mail:Host"];
        var from = configuration["LogicPOS:Mail:From"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            return "O envio de email precisa de LogicPOS:Mail:Host e LogicPOS:Mail:From em appsettings.json.";
        }

        var port = int.TryParse(configuration["LogicPOS:Mail:Port"], out var parsed) ? parsed : 25;
        using var message = new MailMessage
        {
            From = new MailAddress(from),
            Subject = subject,
            Body = body,
            IsBodyHtml = body.Contains("<br", StringComparison.OrdinalIgnoreCase)
        };
        AddAddresses(message.To, to);
        AddAddresses(message.CC, cc);
        AddAddresses(message.Bcc, bcc);
        foreach (var documentId in documentIds)
        {
            var path = await CreateA4FileAsync(documentId, cancellationToken);
            if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
            {
                return "Não foi possível criar o PDF do documento.";
            }

            message.Attachments.Add(new Attachment(path));
        }

        using var client = new SmtpClient(host, port);
        var user = configuration["LogicPOS:Mail:User"];
        var password = configuration["LogicPOS:Mail:Password"];
        if (string.IsNullOrWhiteSpace(user) == false)
        {
            client.Credentials = new NetworkCredential(user, password);
        }

        client.EnableSsl = string.Equals(configuration["LogicPOS:Mail:Ssl"], "true", StringComparison.OrdinalIgnoreCase);
        await client.SendMailAsync(message, cancellationToken);
        return null;
    }

    private static void AddAddresses(MailAddressCollection target, string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        foreach (var address in raw.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            target.Add(address);
        }
    }

    public async Task<IReadOnlyList<PosVatOption>> ListVatRatesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var rates = await database.VatRates
            .AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Select(item => new { item.Id, item.Designation, item.Value })
            .ToListAsync(cancellationToken);
        return rates
            .OrderBy(item => item.Value)
            .Select(item => new PosVatOption(item.Id, $"{item.Designation} ({item.Value:0.##}%)", item.Value))
            .ToList();
    }

    public async Task<IReadOnlyList<PosLookupItem>> ListSourceDocumentsAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var documents = await database.Documents
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.IsDraft == false && item.Status != "A")
            .OrderByDescending(item => item.CreatedAt)
            .Take(100)
            .Select(item => new { item.Id, item.Type, item.Number })
            .ToListAsync(cancellationToken);
        return documents
            .Select(item => new PosLookupItem(item.Id, $"{item.Type} {item.Number}"))
            .ToList();
    }

    public async Task<IReadOnlyList<PosCopiedLine>> LoadDocumentLinesAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var stored = LocalDraftStore.Lines(documentId);
        if (stored is not null)
        {
            return stored;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var rows = await database.DocumentDetails
            .AsNoTracking()
            .Where(item => item.DocumentId == documentId && item.IsDeleted == false)
            .OrderBy(item => item.Order)
            .Select(item => new
            {
                item.ArticleId,
                item.Code,
                item.Designation,
                item.Quantity,
                item.Price,
                item.Discount,
                VatRateId = item.Tax.TaxId,
                VatPercentage = item.Tax.Percentage,
                item.SerialNumber
            })
            .ToListAsync(cancellationToken);
        return rows
            .Select(item => new PosCopiedLine(
                item.ArticleId,
                item.Code,
                item.Designation,
                item.Quantity,
                item.Price,
                item.Discount,
                item.VatRateId,
                item.VatPercentage,
                item.SerialNumber))
            .ToList();
    }

    public async Task<PosDocumentResult> IssueDocumentAsync(
        string documentType,
        Guid customerId,
        IReadOnlyList<PosSaleLine> lines,
        PosDocumentHeader? header = null,
        CancellationToken cancellationToken = default)
    {
        if (lines.Count == 0)
        {
            return PosDocumentResult.Fail("O documento não tem artigos.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var paymentToken = string.IsNullOrWhiteSpace(header?.PaymentToken) ? "MONEY" : header.PaymentToken;
        var paymentMethod = await database.PaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(method => method.IsDeleted == false && (method.Token == paymentToken || method.Acronym == paymentToken), cancellationToken);
        if (paymentMethod is null)
        {
            return PosDocumentResult.Fail("Método de pagamento não encontrado.");
        }

        var currencyId = header?.CurrencyId is Guid selectedCurrency && selectedCurrency != Guid.Empty
            ? selectedCurrency
            : (Guid?)null;
        var exchangeRate = 1m;
        if (currencyId is Guid currencyToUse)
        {
            var currency = await database.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == currencyToUse, cancellationToken);
            if (currency is not null && currency.ExchangeRate > 0)
            {
                exchangeRate = currency.ExchangeRate;
            }
        }

        var details = lines.Select(line => new CreateDocumentDetailDto(
            ArticleId: line.ArticleId,
            Quantity: line.Quantity,
            Price: line.NetUnitPrice,
            VatExemptionId: null,
            VatRateId: line.VatRateId == Guid.Empty ? null : line.VatRateId,
            Discount: line.Discount,
            PriceType: null,
            Notes: line.Notes,
            Country2Code: "PT",
            SerialNumber: line.SerialNumber)).ToList();

        var dto = new CreateDocumentDto(
            Type: documentType,
            CustomerId: customerId,
            PaymentConditionId: header?.PaymentConditionId is Guid paymentCondition && paymentCondition != Guid.Empty ? paymentCondition : null,
            CurrencyId: currencyId,
            ParentId: header?.ParentDocumentId is Guid parent && parent != Guid.Empty ? parent : null,
            ShipToAddress: header?.ShipTo is null ? null : PosDocumentRules.ToAddress(header.ShipTo),
            ShipFromAddress: header?.ShipFrom is null ? null : PosDocumentRules.ToAddress(header.ShipFrom),
            Discount: 0,
            Customer: null,
            ExchangeRate: exchangeRate,
            TotalDelivery: null,
            Notes: string.IsNullOrWhiteSpace(header?.Notes) ? null : header!.Notes.Trim(),
            Details: details,
            PaymentMethods: new[] { new CreateDocumentPaymentMethodDto(paymentMethod.Id, 0) },
            IsDraft: header?.IsDraft == true);

        var references = new DocumentReferences(
            scope.ServiceProvider.GetRequiredService<IDocumentRepository>(),
            scope.ServiceProvider.GetRequiredService<IPaymentMethodRepository>(),
            scope.ServiceProvider.GetRequiredService<IDocumentSeriesRepository>(),
            scope.ServiceProvider.GetRequiredService<ICurrencyRepository>(),
            scope.ServiceProvider.GetRequiredService<ICustomerRepository>(),
            scope.ServiceProvider.GetRequiredService<IPaymentConditionRepository>(),
            scope.ServiceProvider.GetRequiredService<IArticleRepository>(),
            scope.ServiceProvider.GetRequiredService<IVatRateRepository>(),
            scope.ServiceProvider.GetRequiredService<IVatExemptionReasonRepository>());

        var created = await Document.CreateAsync(
            dto,
            references.DocumentRepository,
            references,
            scope.ServiceProvider.GetRequiredService<IDocumentHasher>(),
            FiscalCountry.Portugal,
            terminalId: null,
            cancellationToken);
        if (created.IsFailure)
        {
            return PosDocumentResult.Fail(Explain(created.Error.Message));
        }

        var document = created.Value!;
        await database.Documents.AddAsync(document, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return PosDocumentResult.Ok(document.Id, document.Number);
    }

    public async Task<string?> CreateA4FileAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var document = await database.Documents
            .AsNoTracking()
            .Where(item => item.Id == documentId)
            .Select(doc => new PdfDocument
            {
                Type = doc.Type,
                Status = doc.Status,
                IsDraft = doc.IsDraft,
                CreatedAt = doc.CreatedAt,
                Number = doc.Number,
                TypeName = doc.Series!.DocumentType!.Designation,
                Customer = doc.Customer,
                CustomerId = doc.CustomerId,
                Currency = doc.Currency!.Acronym,
                Hash = doc.Hash,
                Discount = doc.Discount,
                TotalDiscount = doc.TotalDiscount,
                TotalFinal = doc.TotalFinal,
                TotalNet = doc.TotalNet,
                TotalTax = doc.TotalTax,
                WithholdingTaxAmount = doc.WithholdingTaxAmount,
                Notes = doc.Notes,
                AtQRCode = doc.ATQRCode,
                ShipFromAddress = doc.ShipFromAddress,
                ShipToAddress = doc.ShipToAddress,
                PaymentCondition = doc.PaymentConditionId != null ? doc.PaymentCondition!.Designation : null,
                PaymentMethods = doc.PaymentMethods!.Select(payment => new PdfPaymentMethod
                {
                    Designation = payment.PaymentMethod!.Designation,
                    Acronym = payment.PaymentMethod.Acronym,
                    Amount = payment.Amount
                }).ToList(),
                Details = doc.Details!.Select(detail => new PdfDocumentDetail
                {
                    CreatedAt = detail.CreatedAt,
                    Order = detail.Order,
                    Code = detail.Code,
                    Designation = detail.Designation,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    Price = detail.Price,
                    TotalNet = detail.TotalNet,
                    TotalTax = detail.TotalTax,
                    Tax = detail.Tax,
                    TotalFinal = detail.TotalFinal,
                    VatExemptionReason = detail.VatExemptionReason,
                    ArticleClassAcronym = detail.Article!.Class!.Acronym,
                    Notes = detail.Notes,
                    Discount = detail.Discount,
                    SerialNumber = detail.SerialNumber
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (document is null)
        {
            return null;
        }

        document.CustomerOrder = await database.Customers
            .AsNoTracking()
            .Where(customer => customer.Id == document.CustomerId)
            .Select(customer => customer.Order)
            .FirstOrDefaultAsync(cancellationToken);
        document.Details = document.Details.OrderBy(detail => detail.CreatedAt).ToList();
        document.RelatedDocuments = new List<string>();

        var data = new LogicPOS.Application.Features.Finance.Documents.PdfGeneration.DocumentPdfData
        {
            Company = await ReadCompanyAsync(database, cancellationToken),
            Country = FiscalCountry.Portugal,
            Document = document,
            SoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0"
        };
        var mark = FiscalMarks.Read(scope.ServiceProvider.GetService<IFiscalModule>(), new FiscalDocument
        {
            Type = document.Type,
            Number = document.Number,
            Hash = document.Hash,
            QrPayload = document.AtQRCode,
            TotalNet = document.TotalNet,
            TotalTax = document.TotalTax,
            TotalFinal = document.TotalFinal,
            Date = document.CreatedAt,
            CustomerFiscalNumber = document.Customer?.FiscalNumber,
            CountryCode = data.Company.CountryCode2
        });
        if (string.IsNullOrWhiteSpace(mark.CodeLine) == false)
        {
            document.FiscalCodeLine = mark.CodeLine;
        }

        if (string.IsNullOrWhiteSpace(mark.QrPayload) == false)
        {
            document.AtQRCode = mark.QrPayload;
        }

        var safeNumber = string.Concat(document.Number.Select(character => Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));
        var path = Path.Combine(Path.GetTempPath(), $"logicpos-{safeNumber}.pdf");
        await File.WriteAllBytesAsync(path, A4DocumentPdf.Render(data), cancellationToken);
        var printer = scope.ServiceProvider.GetService<ITicketPrinter>();
        if (printer is not null)
        {
            var lines = document.Details.Select(detail => $"{detail.Designation}  {detail.Quantity:0.##}  {detail.TotalFinal:0.00}").ToList();
            lines.Add($"Total {document.TotalFinal:0.00}");
            await printer.TryPrintAsync(document.Number, lines, cancellationToken);
        }

        return path;
    }

    private static async Task<CompanyInformation> ReadCompanyAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var parameters = await database.PreferenceParameters
            .AsNoTracking()
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);
        string? Value(string token) => parameters.FirstOrDefault(item => item.Token == token)?.Value;
        return new CompanyInformation
        {
            Name = Value("COMPANY_NAME"),
            BusinessName = Value("COMPANY_BUSINESS_NAME"),
            CommercialName = Value("TICKET_PRINT_COMERCIAL_NAME"),
            LogoPng = Value("REPORT_FILENAME_LOGO"),
            LogoBmp = Value("TICKET_FILENAME_LOGO"),
            Address = Value("COMPANY_ADDRESS"),
            City = Value("COMPANY_CITY"),
            PostalCode = Value("COMPANY_POSTALCODE"),
            CountryCode2 = Value("COMPANY_COUNTRY_CODE2"),
            Phone = Value("COMPANY_TELEPHONE"),
            MobilePhone = Value("COMPANY_MOBILEPHONE"),
            Email = Value("COMPANY_EMAIL"),
            Website = Value("COMPANY_WEBSITE"),
            FiscalNumber = Value("COMPANY_FISCALNUMBER"),
            StockCapital = Value("COMPANY_STOCK_CAPITAL"),
            DocumentFinalLine1 = Value("REPORT_FOOTER_LINE1"),
            DocumentFinalLine2 = Value("REPORT_FOOTER_LINE2"),
            TaxEntity = Value("COMPANY_TAX_ENTITY"),
            Fax = Value("COMPANY_FAX"),
            TicketFinalLine1 = Value("TICKET_FOOTER_LINE1"),
            TicketFinalLine2 = Value("TICKET_FOOTER_LINE2"),
            CurrencyCode = Value("SYSTEM_CURRENCY")
        };
    }
}
