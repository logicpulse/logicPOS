using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesBySubfamily;

public record SalesBySubfamilyReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public string SubfamilyCode { get; set; } = null!;
    public string FamilyCode { get; set; } = null!;
    public string Family { get; set; } = null!;
    public string Subfamily { get; set; } = null!;
    public DateTime MovementStartTime { get; set; }
    public string ArticleCode { get; set; } = null!;
    public string DocumentNumber { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}

