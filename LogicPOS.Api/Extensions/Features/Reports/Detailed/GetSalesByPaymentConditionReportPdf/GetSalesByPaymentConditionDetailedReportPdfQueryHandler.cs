using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionDetailedReportPdf
{
    public class GetSalesByPaymentConditionDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentConditionDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByPaymentConditionDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByPaymentConditionDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-payment-condition/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}