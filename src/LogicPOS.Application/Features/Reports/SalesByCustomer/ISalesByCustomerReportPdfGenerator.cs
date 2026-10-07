namespace LogicPOS.Application.Features.Reports.SalesByCustomer;

public interface ISalesByCustomerReportPdfGenerator
{
    byte[] GeneratePdf(SalesByCustomerReportData data);
}
