using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Stocks;

public record StockReportData : ReportData
{
    public List<Article> Articles { get; set; } = null!;
}

public record Article
{
    public string Code { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public string Warehouse { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string? SerialNumber { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public string? Customer { get; set; }
    public decimal Price { get; set; }
    public decimal PurchasePrice { get; set; }
    public string? Unit { get; set; }
    public string Family { get; set; } = null!;
    public decimal MinimumStock { get; set; }
    public string Subfamily { get; set; } = null!;
    public string? DocumentNumber { get; set; }
    public decimal TotalStock { get; set; }
}
