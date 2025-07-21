using ErrorOr;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class GetArticlesHistoriesQueryHandler :
        RequestHandler<GetArticlesHistoriesQuery, ErrorOr<PaginatedResult<ArticleHistory>>>
    {
        public GetArticlesHistoriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<PaginatedResult<ArticleHistory>>> Handle(GetArticlesHistoriesQuery query,
                                                                          CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<PaginatedResult<ArticleHistory>>($"articles/stocks/histories{query.GetUrlQuery()}", cancellationToken);
        }
    }
}
