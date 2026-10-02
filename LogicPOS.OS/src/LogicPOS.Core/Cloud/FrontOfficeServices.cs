namespace LogicPOS.Core.FrontOffice;

public interface IPosCatalogService
{
    Task<PosCatalog> LoadAsync(CancellationToken cancellationToken = default);
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
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ListRecentAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosDocumentRow>> ListByPeriodAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosDocumentTypeOption>> ListDocumentTypesAsync(CancellationToken cancellationToken = default);

    Task<string> GetSeriesLabelAsync(string documentType, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListPaymentConditionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<PosCompanyAddress> GetCompanyAddressAsync(CancellationToken cancellationToken = default);

    Task DeleteDraftAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<string?> CancelDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default);

    Task<string?> SendDocumentsByEmailAsync(IReadOnlyList<Guid> documentIds, string to, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosVatOption>> ListVatRatesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosLookupItem>> ListSourceDocumentsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PosCopiedLine>> LoadDocumentLinesAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<PosDocumentResult> IssueDocumentAsync(
        string documentType,
        Guid customerId,
        IReadOnlyList<PosSaleLine> lines,
        PosDocumentHeader? header = null,
        CancellationToken cancellationToken = default);

    Task<string?> CreateA4FileAsync(Guid documentId, CancellationToken cancellationToken = default);
}

public sealed class PosWorkSessionState
{
    public bool DayOpen { get; init; }

    public bool TerminalOpen { get; init; }
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

    Task CloseAsync(Guid orderId, CancellationToken cancellationToken = default);
}
