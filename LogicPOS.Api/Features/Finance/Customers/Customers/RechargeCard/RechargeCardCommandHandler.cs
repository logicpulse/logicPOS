using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.RechargeCard
{
    public class RechargeCardCommandHandler :
        RequestHandler<RechargeCardCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedCache;
        public RechargeCardCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache keyedMemoryCache) : base(factory)
        {
            _keyedCache = keyedMemoryCache;
        }

        public override async Task<ErrorOr<Success>> Handle(RechargeCardCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandlePostCommandAsync<object>($"customers/{command.CustomerId}/recharge-card", command, cancellationToken);

            if (result.IsError)
            {
               return result.Errors;
            }

            CustomersCache.Clear(_keyedCache);
            return Result.Success;
        }


    }
}
