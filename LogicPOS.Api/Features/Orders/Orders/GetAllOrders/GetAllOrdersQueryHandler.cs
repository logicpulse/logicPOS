using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.GetAllOrders
{
    public class GetAllOrdersQueryHandler : RequestHandler<GetAllOrdersQuery, ErrorOr<IEnumerable<Order>>>
    {
        public GetAllOrdersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<Order>>> Handle(GetAllOrdersQuery query,
                                                                       CancellationToken cancellationToken = default)
        {
            var endpoint = "orders" + (query.TableId.HasValue ? $"?tableId={query.TableId}" : "");
            return await HandleGetAllQueryAsync<Order>(endpoint, cancellationToken);
        }
    }
}
