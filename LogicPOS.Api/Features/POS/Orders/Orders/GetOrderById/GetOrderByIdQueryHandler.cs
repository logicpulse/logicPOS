using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.POS.Orders.Orders.GetOrderById
{
    public class GetOrderByIdQueryHandler : RequestHandler<GetOrderByIdQuery, ErrorOr<Order>>
    {
        public GetOrderByIdQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<Order>> Handle(GetOrderByIdQuery request, CancellationToken ct = default)
        {
            return await HandleGetEntityQueryAsync<Order>($"orders/{request.Id}", ct);
        }
    }
}
