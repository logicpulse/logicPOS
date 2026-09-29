using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.ExportCustomersToExcel
{
    public class ExportCustomersToExcelQueryHandler :
        RequestHandler<ExportCustomersToExcelQuery, ErrorOr<TempFile>>
    {
        public ExportCustomersToExcelQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(ExportCustomersToExcelQuery query,
                                                           CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"customers/export-to-excel");
        }
    }
}
