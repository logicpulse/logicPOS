using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentorderdetail")]
public class OrderDetail : Entity.WithCode.AndOrder.AndDesignation
{
    public Article? Article { get; set; }
    public Guid ArticleId { get; set; }

    public VatExemptionReason? VatExemptionReason { get; set; }
    public Guid? VatExemptionReasonId { get; set; }

    public Ticket? Ticket { get; set; }
    public Guid TicketId { get; set; }

    public string Unit { get; set; } = null!;
    [Column(TypeName = "decimal(18,6)")] public decimal Quantity { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal Price { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal Discount { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal Vat { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal TotalDiscount { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal TotalTax { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal TotalFinal { get; set; }
}