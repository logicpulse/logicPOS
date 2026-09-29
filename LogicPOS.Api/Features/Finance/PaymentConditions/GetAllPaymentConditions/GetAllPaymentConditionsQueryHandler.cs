using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition
{
    public class GetAllPaymentConditionsQueryHandler :
        RequestHandler<GetAllPaymentConditionsQuery, ErrorOr<IEnumerable<PaymentCondition>>>
    {
        public GetAllPaymentConditionsQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PaymentCondition>>> Handle(GetAllPaymentConditionsQuery query,
                                                                                  CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetListQueryAsync<PaymentCondition>("payment/conditions", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
        }
    }
}
