using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetCustomerReportPdf
{
    public class GetCustomerReportPdfQueryHandler :
        RequestHandler<GetCustomerReportPdfQuery, ErrorOr<string>>
    {
        public GetCustomerReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetCustomerReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/ofcustomer/pdf");
        }
    }
}
