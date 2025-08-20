using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation
{
    public class UpdateWarehouseLocationCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public string NewDesignation { get; set; }
        public bool IsDefault { get; set; }
    }
}
