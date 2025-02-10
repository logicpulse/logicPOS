using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetAllWarehouseArticles
{
    public class GetAllWarehouseArticlesQueryHandler :
        RequestHandler<GetAllWarehouseArticlesQuery, ErrorOr<IEnumerable<WarehouseArticle>>>
    {
        public GetAllWarehouseArticlesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<WarehouseArticle>>> Handle(GetAllWarehouseArticlesQuery query,
                                                                            CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<WarehouseArticle>("articles/stocks", cancellationToken);
        }
    }
}
