using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticleById
{
    public class GetArticleByIdQueryHandler :
        RequestHandler<GetArticleByIdQuery, ErrorOr<Article>>
    {
        public GetArticleByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Article>> Handle(GetArticleByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Article>($"articles/{query.Id}", cancellationToken);
        }
    }
}
