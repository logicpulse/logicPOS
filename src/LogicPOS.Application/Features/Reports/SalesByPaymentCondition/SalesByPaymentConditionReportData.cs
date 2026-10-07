using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByPaymentCondition;

public record SalesByPaymentConditionReportData  : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public List<SaleItem>? Items { get; set; }
    public string PaymentCondition { get; set; } = null!;
    public string PaymentConditionCode { get; set; } = null!;
    public DateTime MovementStartTime { get; set; }
    public string DocumentNumber { get; set; } = null!;
    public decimal TotalQuantity => Items!.Sum(x => x.Quantity);
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}

public record SaleItem
{
    public string Code { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
}