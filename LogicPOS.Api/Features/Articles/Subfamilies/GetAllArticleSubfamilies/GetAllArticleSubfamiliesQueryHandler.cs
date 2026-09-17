using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies
{
    public class GetAllArticleSubfamiliesQueryHandler :
        RequestHandler<GetAllArticleSubfamiliesQuery, ErrorOr<IEnumerable<ArticleSubfamily>>>
    {
        public GetAllArticleSubfamiliesQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleSubfamily>>> Handle(GetAllArticleSubfamiliesQuery request,
                                                                            CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            string endpoint = $"articles/subfamilies{request.GetUrlQuery()}";
            return await HandleGetListQueryAsync<ArticleSubfamily>(endpoint, cancellationToken,cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
