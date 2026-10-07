using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByEmployee;

public record SalesByEmployeeReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public List<SaleItem>? Items { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public Guid EmployeeId { get; set; }
    public string Employee { get; set; } = null!;
    public DateTime Date { get; set; }
    public string DocumentNumber { get; set; } = null!;
    public decimal TotalQuantity => Items?.Sum(item => item.Quantity) ?? 0;
    public string CustomerName { get; set; } = null!;
    public string CustomerFiscalNumber { get; set; } = null!;
    public decimal Discount { get; set; }
}

public record SaleItem
{
    public string Code { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}
