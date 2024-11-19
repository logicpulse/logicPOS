using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.CloseOrder
{
    public class CloseOrderCommandHandler :
        RequestHandler<CloseOrderCommand, ErrorOr<Unit>>
    {
        public CloseOrderCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CloseOrderCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"orders/{command.OrderId}", cancellationToken);
        }
    }
}
