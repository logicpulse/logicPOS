using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCommissionReportPdf
{
    public class GetSalesByCommissionReportPdfQueryHandler :
        RequestHandler<GetSalesByCommissionReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByCommissionReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByCommissionReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-commission/pdf{query.GetUrlQuery()}");
        }
    }
}
