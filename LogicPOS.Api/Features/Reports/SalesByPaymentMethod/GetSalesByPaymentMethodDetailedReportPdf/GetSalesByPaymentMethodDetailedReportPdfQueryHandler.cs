using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodDetailedReportPdf
{
    public class GetSalesByPaymentMethodDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByPaymentMethodDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByPaymentMethodDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByPaymentMethodDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-payment-method/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}