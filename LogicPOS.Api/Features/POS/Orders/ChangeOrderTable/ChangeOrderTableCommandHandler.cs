using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.ChangeOrderTable
{
    public class ChangeOrderTableCommandHandler :
        RequestHandler<ChangeOrderTableCommand, ErrorOr<Success>>
    {
        public ChangeOrderTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(ChangeOrderTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"orders/{command.OrderId}/change-table", command, cancellationToken);
        }
    }
}
