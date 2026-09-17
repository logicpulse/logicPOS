using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Articles.ImportArticlesFromExcel
{
    public sealed class ImportArticlesFromExcelCommandHandler
        : RequestHandler<ImportArticlesFromExcelCommand, ErrorOr<ExcelImportResponse>>
    {
        public ImportArticlesFromExcelCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override Task<ErrorOr<ExcelImportResponse>> Handle(
            ImportArticlesFromExcelCommand command,
            CancellationToken cancellationToken = default) =>
            HandlePostFileCommandAsync<ExcelImportResponse>(
                "articles/import-from-excel",
                command.FilePath,
                cancellationToken: cancellationToken);
    }
}
