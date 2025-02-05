using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCountryDetailedReportPdf
{
    public class GetSalesByCountryDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByCountryDetailedReportPdfQuery, ErrorOr<string>>
    {
        public GetSalesByCountryDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetSalesByCountryDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-country/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
