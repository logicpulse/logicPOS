namespace LogicPOS.Application.Features.Reports.SalesByDate;

public interface ISalesByDateDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByDateReportData data);
}
