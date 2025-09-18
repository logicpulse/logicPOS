using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.PaymentConditions;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.UpdatePaymentCondition
{
    public class UpdatePaymentConditionCommandHandler :
        RequestHandler<UpdatePaymentConditionCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdatePaymentConditionCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdatePaymentConditionCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleUpdateCommandAsync($"/payment/conditions/{command.Id}", command, cancellationToken);
            if (result.IsError == false)
            {
                PaymentConditionsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
