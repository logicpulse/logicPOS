using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCustomerDetailedReportPdf
{
    public class GetSalesByCustomerDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByCustomerDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByCustomerDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByCustomerDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-customer/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
