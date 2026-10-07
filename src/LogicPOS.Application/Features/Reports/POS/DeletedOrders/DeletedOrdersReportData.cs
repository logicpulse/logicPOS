using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.POS.DeletedOrders;

public record DeletedOrdersReportData : SalesReportData
{
    public List<DeletedOrderItem> Items { get; set; } = null!;
}

public record DeletedOrderItem
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string User { get; set; } = null!;
    public DateTime DeletedAt { get; set; }
    public string? Reason { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal NetTotal => FinalTotal - TaxTotal;
    public decimal DiscountTotal { get; set; }
    public decimal Discount { get; set; }
}