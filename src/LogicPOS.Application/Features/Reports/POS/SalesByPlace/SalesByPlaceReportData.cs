using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.POS.SalesByPlace;

public record SalesByPlaceReportData : SalesReportData
{
    public List<Order> Orders { get; set; } = null!;
}

public record Order
{
    public string Table { get; set; } = null!;
    public string Place { get; set; } = null!;
    public DateTime Date { get; set; }
    public List<OrderItem> Items { get; set; } = null!;
    public string? DocumentNumber { get; set; }
    public decimal TotalQuantity => Items.Sum(oi => oi.Quantity);
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}

public record OrderItem
{
    public string Designation { get; set; } = null!;
    public string Code { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}