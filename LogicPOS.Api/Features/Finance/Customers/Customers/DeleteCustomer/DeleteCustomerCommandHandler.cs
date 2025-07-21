using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Customers.DeleteCustomer
{
    public class DeleteCustomerCommandHandler :
        RequestHandler<DeleteCustomerCommand, ErrorOr<bool>>
    {
        public DeleteCustomerCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"customers/{command.Id}", cancellationToken);
        }
    }
}
