using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockMovementReportPdf
{
    public class GetStockMovementReportPdfQueryHandler :
        RequestHandler<GetStockMovementsReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetStockMovementReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetStockMovementsReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock-movement/pdf{query.GetUrlQuery()}");
        }
    }
}
