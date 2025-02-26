using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetUniqueArticles;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticleByCode
{
    public class GetUniqueArticlesQueryHandler :
        RequestHandler<GetUniqueArticlesQuery, ErrorOr<IEnumerable<WarehouseArticle>>>
    {
        public GetUniqueArticlesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<WarehouseArticle>>> Handle(GetUniqueArticlesQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<WarehouseArticle>($"articles/{query.Id}/unique-articles", cancellationToken);
        }
    }
}
