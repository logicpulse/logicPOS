using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticleByCode
{
    public class GetArticleByCodeQueryHandler :
        RequestHandler<GetArticleByCodeQuery, ErrorOr<ArticleViewModel>>
    {
        public GetArticleByCodeQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<ArticleViewModel>> Handle(GetArticleByCodeQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<ArticleViewModel>($"articles/code/{query.Code}", cancellationToken);
        }
    }
}
