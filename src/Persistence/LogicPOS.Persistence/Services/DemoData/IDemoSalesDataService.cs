namespace LogicPOS.Persistence.Services.DemoData;

public interface IDemoSalesDataService
{
    /// <summary>
    /// Ensures demo sales documents cover dashboard periods when Demo licence + flag are on.
    /// Pass <paramref name="force"/> after enabling the setting so the year window is rebuilt.
    /// </summary>
    Task<DemoSalesDataRunResult> EnsureAsync(bool force = false, CancellationToken cancellationToken = default);
}
