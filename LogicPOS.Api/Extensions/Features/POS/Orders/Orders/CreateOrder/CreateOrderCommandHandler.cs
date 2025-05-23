using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.CreateOrder
{
    public class CreateOrderCommandHandler :
        RequestHandler<CreateOrderCommand, ErrorOr<Guid>>
    {
        public CreateOrderCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync($"orders",command, cancellationToken);
        }
    }
}
