using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCountryReportPdf
{
    public class GetSalesByCountryReportPdfQueryHandler :
        RequestHandler<GetSalesByCountryReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByCountryReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByCountryReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-country/pdf{query.GetUrlQuery()}");
        }
    }
}
