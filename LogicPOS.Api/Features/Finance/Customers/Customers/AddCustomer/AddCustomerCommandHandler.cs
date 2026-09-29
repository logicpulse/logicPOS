using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.AddCustomer
{
    public class AddCustomerCommandHandler :
        RequestHandler<AddCustomerCommand, ErrorOr<Guid>>
    {
        public AddCustomerCommandHandler(IHttpClientFactory factory, IMemoryCache cache) : base(factory, cache)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var result=await HandleAddCommandAsync("customers",command, cancellationToken);

            if (result.IsError == false)
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
