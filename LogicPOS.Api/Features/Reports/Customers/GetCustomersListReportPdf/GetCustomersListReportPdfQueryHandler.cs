using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetCustomerReportPdf
{
    public class GetCustomersListReportPdfQueryHandler :
        RequestHandler<GetCustomersListReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetCustomersListReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetCustomersListReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/customers/list/pdf");
        }
    }
}
