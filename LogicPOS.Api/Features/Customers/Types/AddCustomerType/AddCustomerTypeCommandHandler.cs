using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.Types.AddCustomerType
{
    public class AddCustomerTypeCommandHandler : RequestHandler<AddCustomerTypeCommand, ErrorOr<Guid>>
    {
        public AddCustomerTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddCustomerTypeCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleAddCommand("customers/types", command, cancellationToken);
        }
    }
}
