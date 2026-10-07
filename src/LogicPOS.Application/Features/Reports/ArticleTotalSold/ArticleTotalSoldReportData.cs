using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.ArticleTotalSold;

public record ArticleTotalSoldReportData : SalesReportData
{
   public List<Sale> Sales { get; set; } = null!;
}


public record Sale
{
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public string Subfamily { get; set; } = null!;
    public string SubfamilyCode { get; set; } = null!;
    public string Family { get; set; } = null!;
    public string FamilyCode { get; set; } = null!;
}
