using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByTax.SalesByTaxGroup;

public record SalesByTaxReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public DateTime Date { get; set; }
    public string ArticleCode { get; set; } = null!;
    public string DocumentNumber { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal TaxPercentage { get; set; }
}