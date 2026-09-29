using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.Types.UpdateCustomerType
{
    public class UpdateCustumerTypeCommandHandler :
        RequestHandler<UpdateCustomerTypeCommand, ErrorOr<Success>>
    {
        public UpdateCustumerTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateCustomerTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"customers/types/{command.Id}", command, cancellationToken);
        }
    }
}
