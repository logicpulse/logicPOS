using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.Warehouses.UpdateWarehouse
{
    public class UpdateWarehouseCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public bool IsDefault{ get; set; }
        public bool IsDeleted { get; set; }
        public string Notes { get; set; }
    }
}
