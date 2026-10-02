namespace LogicPOS.Core.Fiscal;

public sealed class NullFiscalModule : IFiscalModule
{
    public bool IsAvailable => false;

    public Task<string?> ExportSaftAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>("A certificação fiscal não está instalada neste posto.");

    public Task<string?> RegisterSeriesAsync(Guid seriesId, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>("A certificação fiscal não está instalada neste posto.");

    public Task<string?> RequestAgtSeriesAsync(int year, string documentType, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>("A certificação fiscal não está instalada neste posto.");

    public Task<string?> NotifyCancellationAsync(Guid documentId, string reason, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
