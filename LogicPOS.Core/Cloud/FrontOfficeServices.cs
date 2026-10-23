namespace LogicPOS.Core.FrontOffice;

public interface IPosCatalogService
{
    Task<PosCatalog> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the button image (base64) for a family, subfamily, or article on demand.
    /// Results are cached in memory for subsequent calls.
    /// </summary>
    Task<string?> GetButtonImageAsync(ButtonImageKind kind, Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    /// <summary>Loads button images for the ids on screen in one database round.</summary>
    Task<PosButtonImages> GetButtonImagesAsync(
        IReadOnlyCollection<Guid> familyIds,
        IReadOnlyCollection<Guid> subfamilyIds,
        IReadOnlyCollection<Guid> articleIds,
        CancellationToken cancellationToken = default)
        => Task.FromResult(PosButtonImages.Empty);

    /// <summary>Clears the in-memory image cache.</summary>
    void ClearImageCache() { }
}

public sealed class PosButtonImages
{
    public PosButtonImages(
        IReadOnlyDictionary<Guid, string?> families,
        IReadOnlyDictionary<Guid, string?> subfamilies,
        IReadOnlyDictionary<Guid, string?> articles)
    {
        Families = families;
        Subfamilies = subfamilies;
        Articles = articles;
    }

    public IReadOnlyDictionary<Guid, string?> Families { get; }

    public IReadOnlyDictionary<Guid, string?> Subfamilies { get; }

    public IReadOnlyDictionary<Guid, string?> Articles { get; }

    public static PosButtonImages Empty { get; } = new(
        new Dictionary<Guid, string?>(),
        new Dictionary<Guid, string?>(),
        new Dictionary<Guid, string?>());
}

public interface IPosCustomerService
{
    Task<PosCustomer?> GetFinalConsumerAsync(CancellationToken cancellationToken = default);

    Task<PosCustomer?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PosCustomer?> FindByCardAsync(string cardNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosCustomer>> SearchAsync(string text, CancellationToken cancellationToken = default);
}

public interface IPosDocumentService
{
    Task<PosDocumentResult> IssueSimplifiedInvoiceAsync(
        Guid customerId,
        string paymentToken,
        decimal discountPercent,
        decimal? amountDelivered,
        string? notes,
        IReadOnlyList<PosSaleLine> lines,
        string documentType = "FS",
        bool createMissingSeries = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ListRecentAsync(CancellationToken cancellationToken = default);

    Task<PosDocumentPage> ListByPeriodAsync(DateTime start, DateTime end, int page, int pageSize, string? search = null, CancellationToken cancellationToken = default, Guid? customerId = null, string? documentType = null);

    Task<IReadOnlyList<PosDocumentTypeOption>> ListDocumentTypesAsync(CancellationToken cancellationToken = default);

    Task<string> GetSeriesLabelAsync(string documentType, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListPaymentConditionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<PosCompanyAddress> GetCompanyAddressAsync(CancellationToken cancellationToken = default);

    /// <summary>Full company block for document PDF preview (name, logo, contacts).</summary>
    Task<LogicPOS.Shared.Features.Company.CompanyInformation> GetCompanyInformationAsync(CancellationToken cancellationToken = default);

    Task DeleteDraftAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<string?> CancelDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default);

    Task<PosEmailDraft> PrepareDocumentEmailAsync(IReadOnlyList<string> documentNumbers, string? fiscalNumber, CancellationToken cancellationToken = default);

    Task<string?> SendDocumentsByEmailAsync(
        IReadOnlyList<Guid> documentIds,
        string to,
        string subject,
        string body,
        string? cc = null,
        string? bcc = null,
        bool sendReceipts = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosVatOption>> ListVatRatesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListVatExemptionReasonsAsync(CancellationToken cancellationToken = default);

    /// <summary>Regras de código postal das tabelas auxiliares de países.</summary>
    Task<IReadOnlyList<PosCountryPostalRule>> ListCountryPostalRulesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListSourceDocumentsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosCopiedLine>> LoadDocumentLinesAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>Customer of a source document for the New Document copy flow.</summary>
    Task<Guid?> GetDocumentCustomerIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>Header + lines to seed a new document from an existing one.</summary>
    Task<PosDocumentCopySource?> LoadDocumentForCopyAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>Lookup label (type + number) for the Copy combo.</summary>
    Task<PosLookupItem?> GetDocumentLookupAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> IssueDocumentAsync(
        string documentType,
        Guid customerId,
        IReadOnlyList<PosSaleLine> lines,
        PosDocumentHeader? header = null,
        CancellationToken cancellationToken = default);

    Task<string?> CreateA4FileAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<string?> CreateReceiptFileAsync(Guid receiptId, CancellationToken cancellationToken = default);

    /// <summary>Volta a comunicar uma guia já emitida à AT. Devolve o código ou o erro da AT.</summary>
    Task<PosAtSendResult> SendTransportDocumentToAtAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>GTK PosDocumentFinancePrintDialog options for an already-printed document.</summary>
    Task<DocumentPrintDialogOptions?> GetPrintDialogOptionsAsync(Guid documentId, CancellationToken cancellationToken = default);
}

/// <summary>Copy/motive options from the document type (GTK PrintCopies / PrintRequestMotive).</summary>
public sealed class PosAtSendResult
{
    public bool Succeeded { get; init; }

    public string? AtDocCodeId { get; init; }

    public string Message { get; init; } = string.Empty;

    public static PosAtSendResult Ok(string? code) => new()
    {
        Succeeded = string.IsNullOrWhiteSpace(code) == false,
        AtDocCodeId = code,
        Message = string.IsNullOrWhiteSpace(code)
            ? "A AT não devolveu o código."
            : $"Código AT: {code}."
    };

    public static PosAtSendResult Fail(string message) => new()
    {
        Succeeded = false,
        Message = message
    };
}

public sealed class DocumentPrintDialogOptions
{
    public string Number { get; init; } = string.Empty;

    public int PrintCopies { get; init; } = 1;

    public bool PrintRequestMotive { get; init; } = true;
}

public sealed class PosWorkSessionState
{
    public bool DayOpen { get; init; }

    public bool TerminalOpen { get; init; }
}

public sealed class PosCashReportLine
{
    public string Name { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal Total { get; init; }
}

public sealed class PosCashReport
{
    public string Title { get; init; } = string.Empty;

    public string? Subtitle { get; init; }

    public DateTime OpenedAt { get; init; }

    public DateTime? ClosedAt { get; init; }

    public decimal OpeningCash { get; init; }

    public decimal ClosingCash { get; init; }

    public decimal CashIn { get; init; }

    public decimal CashOut { get; init; }

    public IReadOnlyList<PosCashReportLine> Families { get; init; } = [];

    public IReadOnlyList<PosCashReportLine> Payments { get; init; } = [];
}

public interface IPosOrderService
{
    Task<PosWorkSessionState> GetWorkSessionAsync(CancellationToken cancellationToken = default);

    Task<PosDocumentResult> SaveAsync(Guid? orderId, IReadOnlyList<PosStoredLine> lines, Guid? tableId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosOpenOrder>> ListOpenAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosTableSlot>> ListTablesAsync(CancellationToken cancellationToken = default);

    Task<Guid?> GetDefaultTableIdAsync(CancellationToken cancellationToken = default);

    Task<PosDocumentResult> ReserveTableAsync(Guid tableId, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> FreeTableAsync(Guid tableId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosStoredLine>> ReadLinesAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> ReduceLineAsync(Guid orderId, Guid articleId, decimal unitPrice, decimal quantity, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> SetLineRemainingAsync(Guid orderId, Guid articleId, decimal unitPrice, decimal quantity, decimal remainingQuantity, decimal remainingNetUnitPrice, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> ChangeTableAsync(Guid orderId, Guid newTableId, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> MoveLineAsync(Guid orderId, Guid destinationTableId, Guid articleId, decimal unitPrice, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> SplitAsync(Guid orderId, int splitters, IReadOnlyList<PosStoredLine> lines, CancellationToken cancellationToken = default);

    Task<PosCashReport?> GetTerminalCashReportAsync(CancellationToken cancellationToken = default);

    Task<PosCashReport?> GetDayCashReportAsync(CancellationToken cancellationToken = default);

    Task CloseAsync(Guid orderId, CancellationToken cancellationToken = default);
}
