using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.GetAllVatRate
{
    public class GetAllVatRatesQueryHandler :
        RequestHandler<GetAllVatRatesQuery, ErrorOr<IEnumerable<VatRate>>>
    {
        public GetAllVatRatesQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<VatRate>>> Handle(GetAllVatRatesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetListQueryAsync<VatRate>("vat-rates", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
