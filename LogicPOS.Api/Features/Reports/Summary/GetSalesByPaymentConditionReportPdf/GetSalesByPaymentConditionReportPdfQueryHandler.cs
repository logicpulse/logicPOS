using ErrorOr;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionReportPdf
{
    public class GetSalesByPaymentConditionReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentConditionReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByPaymentConditionReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByPaymentConditionReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-paymentcondition/pdf{query.GetUrlQuery()}");
        }
    }
}
