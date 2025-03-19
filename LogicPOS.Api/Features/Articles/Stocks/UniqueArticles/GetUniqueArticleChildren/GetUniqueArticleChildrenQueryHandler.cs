using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetUniqueArticleChildren
{
    public class GetUniqueArticleChildrenQueryHandler :
        RequestHandler<GetUniqueArticleChildrenQuery, ErrorOr<IEnumerable<WarehouseArticle>>>
    {
        public GetUniqueArticleChildrenQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<WarehouseArticle>>> Handle(GetUniqueArticleChildrenQuery query,
                                                                            CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<WarehouseArticle>($"articles/uniques/{query.Id}/children", cancellationToken);
        }
    }
}
