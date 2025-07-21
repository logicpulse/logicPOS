using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Company.GetCompanyCurreny
{
    public class GetCompanyCurrencyQueryHandler :
        RequestHandler<GetCompanyCurrencyQuery, ErrorOr<Currency>>
    {
        public GetCompanyCurrencyQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public async override Task<ErrorOr<Currency>> Handle(GetCompanyCurrencyQuery request, CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetEntityQueryAsync<Currency>("company/currency", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }


    }
}
