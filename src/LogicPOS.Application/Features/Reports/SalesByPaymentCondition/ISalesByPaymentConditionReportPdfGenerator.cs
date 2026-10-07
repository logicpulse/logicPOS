namespace LogicPOS.Application.Features.Reports.SalesByPaymentCondition;

public interface ISalesByPaymentConditionReportPdfGenerator
{
    byte[] GeneratePdf(SalesByPaymentConditionReportData data);
}
