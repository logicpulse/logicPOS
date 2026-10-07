namespace LogicPOS.Application.Features.Reports.SalesByPaymentMethod;

public interface ISalesByPaymentMethodReportPdfGenerator
{
    byte[] GeneratePdf(SalesByPaymentMethodReportData data);
}
