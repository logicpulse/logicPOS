using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Stocks.Movements;

public record StockMovementsReportData : ReportData
{
    public List<StockMovement> Movements { get; set; } = null!;
}

public record StockMovement
{
    public string? DocumentNumber { get; set; }
    public DateTime Date { get; set; }
    public string ArticleCode { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? Customer { get; set; } 
    public string? SerialNumber { get; set; }
    public decimal Price { get; set; }
}
