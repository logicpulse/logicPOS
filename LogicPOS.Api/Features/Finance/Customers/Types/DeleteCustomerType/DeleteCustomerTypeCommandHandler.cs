using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.Types.DeleteCustomerType
{
    public class DeleteCustomerTypeCommandHandler :
        RequestHandler<DeleteCustomerTypeCommand, ErrorOr<bool>>
    {
        public DeleteCustomerTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteCustomerTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"customers/types/{command.Id}", cancellationToken);
        }
    }
}
