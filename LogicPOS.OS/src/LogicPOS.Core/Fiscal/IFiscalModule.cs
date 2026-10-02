namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Optional fiscal port. Certified AT/SAFT/AGT code stays outside LogicPOS.OS.
/// </summary>
public interface IFiscalModule
{
    bool IsAvailable { get; }

    Task<string?> ExportSaftAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    Task<string?> RegisterSeriesAsync(Guid seriesId, CancellationToken cancellationToken = default);

    Task<string?> RequestAgtSeriesAsync(int year, string documentType, CancellationToken cancellationToken = default);

    Task<string?> NotifyCancellationAsync(Guid documentId, string reason, CancellationToken cancellationToken = default);
}
