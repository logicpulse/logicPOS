using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.PaymentMethods;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.UpdatePaymentMethod
{
    public class UpdatePaymentMethodCommandHandler :
        RequestHandler<UpdatePaymentMethodCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdatePaymentMethodCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdatePaymentMethodCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"payment/methods/{command.Id}", command, cancellationToken);
            if (result.IsError == false)
            {
                PaymentMethodsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
