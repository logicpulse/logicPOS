namespace LogicPOS.Application.Features.Import;

public interface IExcelReader
{
    Task<IReadOnlyList<ExcelArticleRow>> ReadArticlesAsync(Stream stream, string fileName, CancellationToken ct = default);
    Task<IReadOnlyList<ExcelCustomerRow>> ReadCustomersAsync(Stream stream, string fileName, CancellationToken ct = default);
}
