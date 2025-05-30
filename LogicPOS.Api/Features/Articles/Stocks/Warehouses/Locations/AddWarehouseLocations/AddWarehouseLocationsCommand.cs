using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Warehouses.Locations.AddWarehouseLocations
{
    public class AddWarehouseLocationsCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public IEnumerable<string> Locations { get; set; }
    }
}
