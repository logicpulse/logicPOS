using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.GetAllCustomers
{
    public class GetAllCustomersQueryHandler :
        RequestHandler<GetAllCustomersQuery, ErrorOr<IEnumerable<Customer>>>
    {
        public GetAllCustomersQueryHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Customer>>> Handle(GetAllCustomersQuery query, CancellationToken cancellationToken = default)
        {
            var cacheOptions = GetCacheOptions();
            return await HandleGetListQueryAsync<Customer>("customers", cancellationToken, cacheOptions);
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions().SetAbsoluteExpiration(global::System.TimeSpan.FromMinutes(10));
        }
    }
}
