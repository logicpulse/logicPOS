namespace LogicPOS.Core.BackOffice;

public interface IDashboardBillingService
{
    Task<DashboardSnapshot> LoadAsync(int year, CancellationToken cancellationToken = default);
}

public interface IBackOfficeListingService
{
    IReadOnlyList<string> PageTitles();

    string ResolveTitle(string title);

    bool HasPage(string title);

    Task<ListingSnapshot> QueryAsync(string title, string? search = null, string? mode = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ListingField>> LoadFieldsAsync(string title, Guid? id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LookupOption>> LookupAsync(string typeName, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> SaveAsync(string title, Guid? id, IReadOnlyDictionary<string, string> fields, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> DeleteAsync(string title, Guid id, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> RunActionAsync(string action, Guid? id, string? extra, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> UndoReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Menu titles the signed-in user may open. Null means the menu stays fully available.
    /// </summary>
    Task<IReadOnlyCollection<string>?> AllowedMenuTitlesAsync(CancellationToken cancellationToken = default);
}
