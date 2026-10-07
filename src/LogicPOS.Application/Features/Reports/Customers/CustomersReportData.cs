using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Customers;

public record CustomersReportData : ReportData
{
    public List<Customer> Customers { get; set; } = null!;
}

public record Customer
{
    public DateTime CreatedAt { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FiscalNumber { get; set; } = null!;
    public decimal Discount { get; set; }
    public string? Telephone { get; set; }
    public string Type { get; set; } = null!;
    public string TypeCode { get; set; } = null!;
}