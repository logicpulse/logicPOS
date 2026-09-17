using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.PaymentConditions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.DeletePaymentCondition
{
    public class DeletePaymentConditionCommandHandler :
        RequestHandler<DeletePaymentConditionCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeletePaymentConditionCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeletePaymentConditionCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            var result= await HandleDeleteCommandAsync($"payment-conditions/{command.Id}", cancellationToken);
            if (result.IsError == false)
            {
                PaymentConditionsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
