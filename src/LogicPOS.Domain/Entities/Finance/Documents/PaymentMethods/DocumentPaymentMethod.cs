using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

public class DocumentPaymentMethod : Entity
{
    public Guid DocumentId { get; set; }
    public Document? Document { get; set; } 

    public Guid PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal Amount { get; set; }
}