using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.PaymentConditions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.AddPaymentCondition
{
    public class AddPaymentConditionCommandHandler : RequestHandler<AddPaymentConditionCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddPaymentConditionCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPaymentConditionCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleAddCommandAsync("payment/conditions", command, cancellationToken);
            if (result.IsError == false)
            {
                PaymentConditionsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
