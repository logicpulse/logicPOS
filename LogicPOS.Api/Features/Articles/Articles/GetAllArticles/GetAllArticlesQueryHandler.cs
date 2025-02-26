using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetAllArticles
{
    public class GetAllArticlesQueryHandler :
        RequestHandler<GetAllArticlesQuery, ErrorOr<IEnumerable<Article>>>
    {
        public GetAllArticlesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Article>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<Article>("articles", cancellationToken);
        }
    }
}
