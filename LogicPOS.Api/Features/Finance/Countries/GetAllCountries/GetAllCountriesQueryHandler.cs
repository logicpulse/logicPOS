using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.GetAllCountries
{
    public class GetAllCountriesQueryHandler : RequestHandler<GetAllCountriesQuery, ErrorOr<IEnumerable<Country>>>
    {

        public GetAllCountriesQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Country>>> Handle(GetAllCountriesQuery query,
                                                                         CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetListQueryAsync<Country>("countries", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
