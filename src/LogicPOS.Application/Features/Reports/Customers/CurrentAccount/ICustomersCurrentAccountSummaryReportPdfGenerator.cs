namespace LogicPOS.Application.Features.Reports.Customers.CurrentAccount;

public interface ICustomersCurrentAccountSummaryReportPdfGenerator
{
    byte[] GeneratePdf(CustomersCurrentAccountSummaryReportData data);
}
