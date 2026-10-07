using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

public class PrinterAssociation : Entity
{
    public Guid PrinterId { get; set; } 
    public Printer? Printer { get; set; }
    public Guid EntityId { get; set; }
}
