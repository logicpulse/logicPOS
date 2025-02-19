using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockByArticleReportPdf
{
    public class GetStockByArticleReportPdfQueryHandler :
        RequestHandler<GetStockByArticleReportPdfQuery, ErrorOr<string>>
    {
        public GetStockByArticleReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetStockByArticleReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock-by-article/pdf{query.GetUrlQuery()}&articleId={query.ArticleId}");
        }
    }
}
