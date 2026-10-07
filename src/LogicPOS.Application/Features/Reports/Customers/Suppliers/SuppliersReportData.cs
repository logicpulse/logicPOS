using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Customers.Suppliers;

public record SuppliersReportData : ReportData
{
    public List<Supplier> Suppliers { get; set; } = null!;
}

public record Supplier
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FiscalNumber { get; set; } = null!;
    public decimal Discount { get; set; }
    public string? Telephone { get; set; }
    public string Country { get; set; } = null!;
}