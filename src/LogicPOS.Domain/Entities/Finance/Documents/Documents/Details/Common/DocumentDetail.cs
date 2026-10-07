using LogicPOS.Domain.ValueObjects;

using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinancedetail")]
public class DocumentDetail : Entity.WithCode.AndOrder.AndDesignation
{
    public Article? Article { get; set; }
    public Guid ArticleId { get; set; }

    public Document? Document { get; set; }
    public Guid DocumentId { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Quantity { get; set; }
    
    public string? Unit { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Price { get; set; }
    public DocumentDetailTax Tax { get; set; } = null!;
    public string? VatExemptionReason { get; set; }
    public string? VatExemptionCode { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal TotalNet { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal TotalDiscount { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal TotalTax { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal TotalFinal { get; set; }
    public Enums.PriceType? PriceType { get; set; }
    public string? SerialNumber { get; set; }

    public string AbbreviatedDesignation => Designation.Length > 50 ? Designation.Substring(0, 50) + "..." : Designation;
}
