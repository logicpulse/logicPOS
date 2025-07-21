using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.GetWarehouseArticleById
{
    public class GetWarehouseArticleByIdQueryHandler :
        RequestHandler<GetWarehouseArticleByIdQuery, ErrorOr<WarehouseArticle>>
    {
        public GetWarehouseArticleByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<WarehouseArticle>> Handle(GetWarehouseArticleByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<WarehouseArticle>($"articles/warehouse-articles/{query.Id}", cancellationToken);
        }
    }
}
