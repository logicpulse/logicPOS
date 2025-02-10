using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class WarehouseLocation : ApiEntity, IWithDesignation
    {
        public Guid WarehouseId { get; set; }
        public string Designation { get; set; }
    }
}
