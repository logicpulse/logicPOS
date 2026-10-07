using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.Import;

public interface ICatalogExcelImportService
{
    Task<Result<ExcelImportResponse>> ImportArticlesAsync(Stream stream, string fileName, CancellationToken ct = default);
    Task<Result<ExcelImportResponse>> ImportCustomersAsync(Stream stream, string fileName, CancellationToken ct = default);
}
