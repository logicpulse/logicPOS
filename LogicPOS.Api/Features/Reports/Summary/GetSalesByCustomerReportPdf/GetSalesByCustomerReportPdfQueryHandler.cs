using ErrorOr;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCustomerReportPdf
{
    public class GetSalesByCustomerReportPdfQueryHandler :
        RequestHandler<GetSalesByCustomerReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByCustomerReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByCustomerReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-customer/pdf{query.GetUrlQuery()}");
        }
    }
}
