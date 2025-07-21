using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockReportPdf
{
    public class GetStockByWarehouseReportPdfQueryHandler :
        RequestHandler<GetStockByWarehouseReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetStockByWarehouseReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetStockByWarehouseReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock/pdf{query.GetUrlQuery()}" +
                                                                    $"&articleId={query.ArticleId}"+
                                                                    $"&warehouseId={query.WarehouseId}"+
                                                                    $"&serialNumber={query.SerialNumber}");
        }
    }
}
