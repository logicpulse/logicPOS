using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.PaymentMethods;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.AddPaymentMethod
{
    public class AddPaymentMethodCommandHandler : RequestHandler<AddPaymentMethodCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddPaymentMethodCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPaymentMethodCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleAddCommandAsync("payment/methods", command, cancellationToken);
            if(result.IsError==false)
            {
                PaymentMethodsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
