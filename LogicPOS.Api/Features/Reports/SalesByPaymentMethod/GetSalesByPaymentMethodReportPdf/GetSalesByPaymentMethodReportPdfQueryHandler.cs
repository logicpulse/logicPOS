using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodReportPdf
{
    public class GetSalesByPaymentMethodReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentMethodReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByPaymentMethodReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByPaymentMethodReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-paymentmethod/pdf{query.GetUrlQuery()}");
        }
    }
}
