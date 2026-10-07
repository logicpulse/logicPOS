namespace LogicPOS.Application.Features.Reports.SalesBySubfamily;

public interface ISalesBySubfamilyDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesBySubfamilyReportData data);
}
