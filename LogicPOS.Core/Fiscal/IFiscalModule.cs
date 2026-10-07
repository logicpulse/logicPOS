namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Optional fiscal port. Authority calls and document marks stay outside LogicPOS.OS.
/// A third party supplies them by dropping a *Plugin.dll into the plugins folder.
/// </summary>
public interface IFiscalModule
{
    bool IsAvailable { get; }

    /// <summary>
    /// Lines and QR payload to print for this document. Empty when this module has nothing to add.
    /// </summary>
    FiscalPrint DescribePrint(FiscalDocument document);

    Task<string?> ExportSaftAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    Task<string?> RegisterSeriesAsync(Guid seriesId, CancellationToken cancellationToken = default);

    Task<string?> RequestSeriesCodeAsync(int year, string documentType, CancellationToken cancellationToken = default);

    Task<string?> NotifyCancellationAsync(Guid documentId, string reason, CancellationToken cancellationToken = default);
}
