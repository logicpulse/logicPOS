using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.PaymentMethods;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentMethods.DeletePaymentMethod
{
    public class DeletePaymentMethodCommandHandler :
        RequestHandler<DeletePaymentMethodCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeletePaymentMethodCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeletePaymentMethodCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleDeleteCommandAsync($"payment-methods/{command.Id}", cancellationToken);
            if (result.IsError == false)
            {
                PaymentMethodsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
