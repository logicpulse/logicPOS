namespace LogicPOS.Application.Features.Reports.SalesByCustomer;

public interface ISalesByCustomerDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByCustomerReportData data);
}
