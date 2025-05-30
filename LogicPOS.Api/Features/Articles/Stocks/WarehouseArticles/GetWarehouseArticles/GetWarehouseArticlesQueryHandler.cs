using ErrorOr;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetWarehouseArticles
{
    public class GetWarehouseArticlesQueryHandler :
        RequestHandler<GetWarehouseArticlesQuery, ErrorOr<PaginatedResult<WarehouseArticleViewModel>>>
    {
        public GetWarehouseArticlesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<PaginatedResult<WarehouseArticleViewModel>>> Handle(GetWarehouseArticlesQuery query,
                                                                                      CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetEntityQueryAsync<PaginatedResult<WarehouseArticleViewModel>>($"articles/warehouse-articles{urlQuery}", cancellationToken);
        }
    }
}
