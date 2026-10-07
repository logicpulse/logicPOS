using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_warehouselocation")]
public class WarehouseLocation : Entity
{
    public string Designation { get; set; } = null!;
    public Warehouse? Warehouse { get; set; } 
    public Guid WarehouseId { get; set; }
    public bool IsDefault { get; set; }
}
