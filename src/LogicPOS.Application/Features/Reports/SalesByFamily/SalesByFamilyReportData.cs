using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByFamily;

public record SalesByFamilyReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public string FamilyCode { get; set; } = null!;
    public string Family { get; set; } = null!;
    public string DocumentNumber { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Code { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public DateTime MovementStartTime { get; set; }
    public decimal FinalTotal { get; set; }
}




