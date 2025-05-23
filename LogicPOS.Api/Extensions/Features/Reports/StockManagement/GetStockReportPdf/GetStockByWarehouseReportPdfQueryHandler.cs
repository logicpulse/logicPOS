using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockReportPdf
{
    public class GetStockByWarehouseReportPdfQueryHandler :
        RequestHandler<GetStockByWarehouseReportPdfQuery, ErrorOr<string>>
    {
        public GetStockByWarehouseReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetStockByWarehouseReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock/pdf{query.GetUrlQuery()}" +
                                                                    $"&articleId={query.ArticleId}"+
                                                                    $"&warehouseId={query.WarehouseId}"+
                                                                    $"&serialNumber={query.SerialNumber}");
        }
    }
}
