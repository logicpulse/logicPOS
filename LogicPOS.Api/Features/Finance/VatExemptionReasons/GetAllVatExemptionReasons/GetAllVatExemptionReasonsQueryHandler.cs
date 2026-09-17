using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons
{
    public class GetAllVatExemptionReasonsQueryHandler :
        RequestHandler<GetAllVatExemptionReasonsQuery, ErrorOr<IEnumerable<VatExemptionReason>>>
    {
        public GetAllVatExemptionReasonsQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<VatExemptionReason>>> Handle(GetAllVatExemptionReasonsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetListQueryAsync<VatExemptionReason>("vat-exemption-reasons", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
