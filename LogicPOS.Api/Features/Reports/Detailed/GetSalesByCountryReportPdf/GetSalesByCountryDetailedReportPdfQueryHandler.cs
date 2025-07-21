using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetSalesByCountryDetailedReportPdf
{
    public class GetSalesByCountryDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByCountryDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByCountryDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByCountryDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-country/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}
