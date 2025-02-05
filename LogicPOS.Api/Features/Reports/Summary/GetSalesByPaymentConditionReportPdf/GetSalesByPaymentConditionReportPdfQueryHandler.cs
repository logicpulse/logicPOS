using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionReportPdf
{
    public class GetSalesByPaymentConditionReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentConditionReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByPaymentConditionReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByPaymentConditionReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-paymentcondition/pdf{query.GetUrlQuery()}");
        }
    }
}
