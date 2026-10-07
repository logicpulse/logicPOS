namespace LogicPOS.Application.Features.Reports.SalesByPaymentMethod;

public interface ISalesByPaymentMethodDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByPaymentMethodReportData data);
}
