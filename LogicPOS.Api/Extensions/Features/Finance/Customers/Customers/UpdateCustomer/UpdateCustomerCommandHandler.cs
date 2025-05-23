using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandHandler :
        RequestHandler<UpdateCustomerCommand, ErrorOr<Unit>>
    {
        public UpdateCustomerCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"customers/{command.Id}", command, cancellationToken);
        }
    }
}
