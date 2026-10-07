using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.CompanyBilling;

public record CompanyBillingReportData : SalesReportData
{
    public List<Invoice> Invoices { get; set; } = null!;
}

public record Invoice
{
    public DateTime Date { get; set; }
    public string Number { get; set; } = null!;
    public string CustomerFiscalNumber { get; set; } = null!;
    public string Customer { get; set; } = null!;
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}