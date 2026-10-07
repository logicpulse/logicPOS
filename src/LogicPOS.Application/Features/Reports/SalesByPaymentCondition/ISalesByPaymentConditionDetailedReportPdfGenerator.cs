namespace LogicPOS.Application.Features.Reports.SalesByPaymentCondition;

public interface ISalesByPaymentConditionDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByPaymentConditionReportData data);
}
