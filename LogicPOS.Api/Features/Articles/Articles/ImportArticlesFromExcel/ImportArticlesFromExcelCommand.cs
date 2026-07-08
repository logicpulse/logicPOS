using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;

namespace LogicPOS.Api.Features.Articles.Articles.ImportArticlesFromExcel
{
    public sealed class ImportArticlesFromExcelCommand : IRequest<ErrorOr<ExcelImportResponse>>
    {
        public ImportArticlesFromExcelCommand(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}
