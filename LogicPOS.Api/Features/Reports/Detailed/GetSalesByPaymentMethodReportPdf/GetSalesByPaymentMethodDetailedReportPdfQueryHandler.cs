using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodDetailedReportPdf
{
    public class GetSalesByPaymentMethodDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentMethodDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByPaymentMethodDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByPaymentMethodDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-payment-method/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}