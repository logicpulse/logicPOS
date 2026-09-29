using ErrorOr;
using LogicPOS.Api.Features.Articles.Articles;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Articles.GetAutoCompleteLines
{
    public class GetAutoCompleteLinesQueryHandler : RequestHandler<GetAutoCompleteLinesQuery, ErrorOr<IEnumerable<AutoCompleteLine>>>
    {
        public GetAutoCompleteLinesQueryHandler(IHttpClientFactory httpFactory, IMemoryCache cache) : base(httpFactory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<AutoCompleteLine>>> Handle(GetAutoCompleteLinesQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<AutoCompleteLine>(
                ArticlesCache.AutocompleteLinesEndpoint,
                cancellationToken,
                GetCacheOptions());
        }

        private static MemoryCacheEntryOptions GetCacheOptions() =>
            new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
    }

}
