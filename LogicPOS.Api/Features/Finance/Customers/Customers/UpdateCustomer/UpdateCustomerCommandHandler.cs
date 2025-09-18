using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Customers.Customers;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandHandler :
        RequestHandler<UpdateCustomerCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedCache;
        public UpdateCustomerCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache keyedMemoryCache) : base(factory)
        {
            _keyedCache = keyedMemoryCache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"customers/{command.Id}", command, cancellationToken);

            if (result.IsError == false)
            {
                CustomersCache.Clear(_keyedCache);
            }

            return result;
        }


    }
}
