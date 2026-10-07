using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.POS.SalesByCommission;

public record SalesByCommissionReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public string? User { get; set; }
    public string? UserCode { get; set; }
    public decimal UserCommission { get; set; }
    public string Article { get; set; } = null!;
    public string ArticleCode { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal NetTotal { get; set; }
}