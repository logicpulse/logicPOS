using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.AddCustomer
{
    public class AddCustomerCommandHandler :
        RequestHandler<AddCustomerCommand, ErrorOr<Guid>>
    {
        public AddCustomerCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCustomerCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("customers",command, cancellationToken);
        }
    }
}
