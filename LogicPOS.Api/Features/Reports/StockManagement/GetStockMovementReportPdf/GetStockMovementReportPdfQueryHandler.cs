using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockMovementReportPdf
{
    public class GetStockMovementReportPdfQueryHandler :
        RequestHandler<GetStockMovementsReportPdfQuery, ErrorOr<string>>
    {
        public GetStockMovementReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetStockMovementsReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock-movement/pdf{query.GetUrlQuery()}");
        }
    }
}
