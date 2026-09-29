using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Warehouses.GetAllWarehouses
{
    public class GetAllWarehousesQueryHandler :
        RequestHandler<GetAllWarehousesQuery, ErrorOr<IEnumerable<Warehouse>>>
    {
        public GetAllWarehousesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Warehouse>>> Handle(GetAllWarehousesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<Warehouse>("articles/stocks/warehouses", cancellationToken);
        }
    }
}
