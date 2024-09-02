using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.Warehouses.UpdateWarehouse
{
    public class UpdateWarehouseCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public string NewDesignation { get; set; }
        public bool IsDefault{ get; set; }
        public IEnumerable<string> Locations { get; set; }
    }
}
