using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.RemoveTicketItem
{
    public class MoveTicketItemCommandHandler :
        RequestHandler<MoveTicketItemCommand, ErrorOr<Success>>
    {
        public MoveTicketItemCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(MoveTicketItemCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"orders/{command.OrderId}/move-item", command, cancellationToken);
        }
    }
}
