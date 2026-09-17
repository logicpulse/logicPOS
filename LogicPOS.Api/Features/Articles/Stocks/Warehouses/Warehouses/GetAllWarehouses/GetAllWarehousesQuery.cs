using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Warehouses.GetAllWarehouses
{
    public class GetAllWarehousesQuery : IRequest<ErrorOr<IEnumerable<Warehouse>>>
    {
    }
}
