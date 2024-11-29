using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Warehouses.DeleteWarehouse
{
    public class DeleteWarehouseCommand : DeleteCommand
    {
        public DeleteWarehouseCommand(Guid id) : base(id)
        {
        }
    }
}
