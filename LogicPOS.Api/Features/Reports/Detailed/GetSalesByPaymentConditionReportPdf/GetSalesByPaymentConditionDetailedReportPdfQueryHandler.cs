using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionDetailedReportPdf
{
    public class GetSalesByPaymentConditionDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentConditionDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByPaymentConditionDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByPaymentConditionDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-payment-condition/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}