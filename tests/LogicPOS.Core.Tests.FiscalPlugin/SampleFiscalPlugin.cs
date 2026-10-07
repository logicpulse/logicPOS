using LogicPOS.Core.Fiscal;

namespace LogicPOS.Core.Tests.FiscalPlugin;

public sealed class SampleFiscalPlugin : IFiscalModule
{
    public bool IsAvailable => true;

    public FiscalPrint DescribePrint(FiscalDocument document)
        => new()
        {
            CodeLine = "MARK " + document.Number,
            QrPayload = "QR " + document.Number
        };

    public Task<string?> ExportSaftAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    public Task<string?> RegisterSeriesAsync(Guid seriesId, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    public Task<string?> RequestSeriesCodeAsync(int year, string documentType, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    public Task<string?> NotifyCancellationAsync(Guid documentId, string reason, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
