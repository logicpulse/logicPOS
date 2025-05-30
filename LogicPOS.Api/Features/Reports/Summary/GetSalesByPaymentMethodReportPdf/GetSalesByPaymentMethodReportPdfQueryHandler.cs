using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodReportPdf
{
    public class GetSalesByPaymentMethodReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentMethodReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByPaymentMethodReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByPaymentMethodReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-paymentmethod/pdf{query.GetUrlQuery()}");
        }
    }
}
