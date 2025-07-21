using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticles
{
    public class GetArticlesQueryHandler :
        RequestHandler<GetArticlesQuery, ErrorOr<PaginatedResult<ArticleViewModel>>>
    {
        public GetArticlesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<PaginatedResult<ArticleViewModel>>> Handle(GetArticlesQuery request,
                                                                                      CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<PaginatedResult<ArticleViewModel>>($"articles{request.GetUrlQuery()}", cancellationToken);
        }
    }
}
