namespace LogicPOS.Application.Features.Reports.SalesByDate;

public interface ISaleByDocumentDateReportPdfGenerator
{
    byte[] GeneratePdf(SalesByDateReportData data);
}
