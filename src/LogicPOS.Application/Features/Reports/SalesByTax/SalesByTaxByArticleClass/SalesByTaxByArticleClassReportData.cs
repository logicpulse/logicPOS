using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByTax.SalesByTaxByArticleClass;

public record SalesByTaxByArticleClassReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal TaxPercentage { get; set; }
    public string ArticleClass { get; set; } = null!;
    public string Currency { get; set; } = null!;
}

