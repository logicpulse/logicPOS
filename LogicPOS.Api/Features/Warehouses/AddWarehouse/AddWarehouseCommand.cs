using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Warehouses.AddWarehouse
{
    public class AddWarehouseCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public bool IsDefault { get; set; }

        public IEnumerable<WarehouseLocation> Locations { get; set; }
    }
}
