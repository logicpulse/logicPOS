using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandHandler :
        RequestHandler<UpdateCustomerCommand, ErrorOr<Unit>>
    {
        public UpdateCustomerCommandHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"customers/{command.Id}", command, cancellationToken);

            if(result.IsError == false)
            {
                RemoveCustomersFromCache();
            }

            return result;
        }

        private void RemoveCustomersFromCache()
        {
            _cache.Remove("customers");
        }
    }
}
