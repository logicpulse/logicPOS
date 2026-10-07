
namespace LogicPOS.Application.Features.Reports.Common;

public abstract record SalesReportData : ReportData
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal NetTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal SalesItemsTotalQuantity { get; set; }

}