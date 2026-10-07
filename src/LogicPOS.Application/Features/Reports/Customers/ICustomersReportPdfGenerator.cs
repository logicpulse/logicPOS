namespace LogicPOS.Application.Features.Reports.Customers;

public interface ICustomersReportPdfGenerator
{
    byte[] GeneratePdf(CustomersReportData data);
}
