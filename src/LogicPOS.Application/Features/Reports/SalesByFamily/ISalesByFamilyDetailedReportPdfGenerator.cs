namespace LogicPOS.Application.Features.Reports.SalesByFamily;

public interface ISalesByFamilyDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByFamilyReportData data);
}
