using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities.POS.Orders.Orders.Common;

public class OrderDocument : Entity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; } 
    public Guid DocumentId { get; set; }
    public Document? Document { get; set; }
}