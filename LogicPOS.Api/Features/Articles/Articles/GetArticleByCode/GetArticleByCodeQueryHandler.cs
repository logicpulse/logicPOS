using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticleByCode
{
    public class GetArticleByCodeQueryHandler :
        RequestHandler<GetArticleByCodeQuery, ErrorOr<Article>>
    {
        public GetArticleByCodeQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Article>> Handle(GetArticleByCodeQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Article>($"articles/code/{query.Code}", cancellationToken);
        }
    }
}
