using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.ImportCustomersFromExcel
{
    public sealed class ImportCustomersFromExcelCommand : IRequest<ErrorOr<ExcelImportResponse>>
    {
        public ImportCustomersFromExcelCommand(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}
