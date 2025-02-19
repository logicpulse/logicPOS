using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.GetStockByArticleGainReportPdf
{
    public class GetStockByArticleGainReportPdfQueryHandler :
        RequestHandler<GetStockByArticleGainReportPdfQuery, ErrorOr<string>>
    {
        public GetStockByArticleGainReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetStockByArticleGainReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/stock-by-article-gain/pdf{query.GetUrlQuery()}" +
                                                                    $"&articleId={query.ArticleId}"+
                                                                    $"&customerId={query.CustomerId}");
        }
    }
}
