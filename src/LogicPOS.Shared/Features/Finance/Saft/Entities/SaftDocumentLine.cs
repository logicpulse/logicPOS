namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftDocumentLine {
    public SaftOrderReference? OrderReference { get; set; }
    public string ProductCode { get; init; } = null!;
    public string ProductDescription { get; init; } = null!;
    public decimal Quantity { get; init; } 
    public string UnitOfMeasure { get; init; } = null!;
    public decimal UnitPrice => Price - TotalDiscount / (Quantity == 0 ? 1 : Quantity);
    public SaftDocumentLineReference? Reference { get; set; }
    public string Description { get; init; } = null!;
    public decimal Amount { get; set; }
    public SaftTax Tax { get; init; } = null!;
    public string? TaxExemptionReason { get; init; } 
    public string? TaxExemptionCode { get; init; }
    public decimal SettlementAmount { get; init; } 
    public decimal TaxPayable => Amount * (Tax.Percentage / 100);
    public decimal Price { get; init; }
    public decimal TotalDiscount { get; init; }
}