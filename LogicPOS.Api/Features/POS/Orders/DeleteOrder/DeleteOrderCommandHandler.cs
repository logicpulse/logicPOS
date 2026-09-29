using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.POS.Orders.Orders.DeleteOrder
{
    public class DeleteOrderCommandHandler :
        RequestHandler<DeleteOrderCommand, ErrorOr<bool>>
    {
        public DeleteOrderCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteOrderCommand command, CancellationToken ct = default)
        {
            return await HandleDeleteCommandAsync($"orders/{command.OrderId}{command.GetUrlQuery()}", ct);
        }
    }
}
