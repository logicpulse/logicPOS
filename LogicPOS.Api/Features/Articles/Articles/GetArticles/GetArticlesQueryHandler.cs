using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticles
{
    public class GetArticlesQueryHandler :
        RequestHandler<GetArticlesQuery, ErrorOr<PaginatedResult<ArticleViewModel>>>
    {
        public GetArticlesQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<PaginatedResult<ArticleViewModel>>> Handle(GetArticlesQuery request,
                                                                                      CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            string endpoint = $"articles{request.GetUrlQuery()}";
            return await HandleGetQueryAsync<PaginatedResult<ArticleViewModel>>(endpoint, cancellationToken, cacheOptions);
        }
        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
