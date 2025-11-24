using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Articles.GetArticleViewModel
{
    public class GetArticleViewModelQueryHandler :
        RequestHandler<GetArticleViewModelQuery, ErrorOr<ArticleViewModel>>
    {
        public GetArticleViewModelQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<ArticleViewModel>> Handle(GetArticleViewModelQuery query, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<ArticleViewModel>($"articles/{query.Id}/view", ct);
        }
    }
}
