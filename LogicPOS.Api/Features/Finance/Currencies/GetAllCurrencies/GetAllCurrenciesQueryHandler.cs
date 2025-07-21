using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.GetAllCurrencies
{
    public class GetAllCurrenciesQueryHandler :
        RequestHandler<GetAllCurrenciesQuery, ErrorOr<IEnumerable<Currency>>>
    {
        public GetAllCurrenciesQueryHandler(IHttpClientFactory httpClientFactory, IMemoryCache cache) : base(httpClientFactory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Currency>>> Handle(GetAllCurrenciesQuery request,
                                                                    CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetListQueryAsync<Currency>("currencies", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
