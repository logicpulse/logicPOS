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
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public bool IsDefault{ get; set; }
        public bool IsDeleted { get; set; }
        public string NewNotes { get; set; }
    }
}
