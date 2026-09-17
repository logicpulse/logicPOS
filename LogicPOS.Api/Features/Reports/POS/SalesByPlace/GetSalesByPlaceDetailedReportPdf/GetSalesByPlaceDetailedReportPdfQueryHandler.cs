using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.POS.SalesByPlace.GetSalesByPlaceDetailedReportPdf
{
    public class GetSalesByPlaceDetailedReportPdfQueryHandler :
        RequestHandler<GetSalesByPlaceDetailedReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSalesByPlaceDetailedReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSalesByPlaceDetailedReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/sales-by-place/detailed/pdf{query.GetUrlQuery()}");
        }
    }
}