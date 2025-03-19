using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation
{
    public class UpdateWarehouseLocationCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public string NewDesignation { get; set; }
    }
}
