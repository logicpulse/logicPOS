using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Orders.GetAllOrders
{
    public class GetOpenOrdersQueryHandler : RequestHandler<GetOpenOrdersQuery, ErrorOr<IEnumerable<Order>>>
    {
        public GetOpenOrdersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<Order>>> Handle(GetOpenOrdersQuery query,
                                                                       CancellationToken cancellationToken = default)
        {
            var endpoint = "orders" + (query.TableId.HasValue ? $"?tableId={query.TableId}" : "");
            return await HandleGetListQueryAsync<Order>(endpoint, cancellationToken);
        }
    }
}
