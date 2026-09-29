using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.ReduceItems
{
    public class ReduceOrderItemsCommandHandler :
        RequestHandler<ReduceOrderItemsCommand, ErrorOr<Success>>
    {
        public ReduceOrderItemsCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(ReduceOrderItemsCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"orders/{command.OrderId}/reduce-items", command, cancellationToken);
        }
    }
}
