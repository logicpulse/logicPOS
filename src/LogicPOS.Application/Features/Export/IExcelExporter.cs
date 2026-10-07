namespace LogicPOS.Application.Features.Articles.Exporters;

public interface IExcelExporter
{
    public Task<byte[]> ExportAsync<T>(IEnumerable<T> lines, string workSheetName, CancellationToken ct = default);
}