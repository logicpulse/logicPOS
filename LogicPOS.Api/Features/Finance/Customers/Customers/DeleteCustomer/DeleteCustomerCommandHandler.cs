using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Customers.Customers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.DeleteCustomer
{
    public class DeleteCustomerCommandHandler :
        RequestHandler<DeleteCustomerCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteCustomerCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleDeleteCommandAsync($"customers/{command.Id}", cancellationToken);

            if (result.IsError == false)
            {
                CustomersCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
