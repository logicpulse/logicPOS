using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByTax.SalesByTaxByArticleType;

public record SalesByTaxByArticleTypeReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public string DocumentType { get; set; } = null!;
    public string DocumentName => DocumentReportsUtils.GetDocumentNameFromType(DocumentType);
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal TaxPercentage { get; set; }
    public string Currency { get; set; } = null!;
}

