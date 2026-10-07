namespace LogicPOS.Application.Features.Reports.POS.SalesByCommission;

public interface ISalesByCommissionReportPdfGenerator
{
    byte[] GeneratePdf(SalesByCommissionReportData data);
}
