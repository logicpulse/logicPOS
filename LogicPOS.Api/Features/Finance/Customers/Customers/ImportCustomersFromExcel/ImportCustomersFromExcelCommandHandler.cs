using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.ImportCustomersFromExcel
{
    public sealed class ImportCustomersFromExcelCommandHandler
        : RequestHandler<ImportCustomersFromExcelCommand, ErrorOr<ExcelImportResponse>>
    {
        public ImportCustomersFromExcelCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override Task<ErrorOr<ExcelImportResponse>> Handle(
            ImportCustomersFromExcelCommand command,
            CancellationToken cancellationToken = default) =>
            HandlePostFileCommandAsync<ExcelImportResponse>(
                "customers/import-from-excel",
                command.FilePath,
                cancellationToken: cancellationToken);
    }
}
