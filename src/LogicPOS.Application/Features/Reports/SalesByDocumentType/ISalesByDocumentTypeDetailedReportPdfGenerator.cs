namespace LogicPOS.Application.Features.Reports.SalesByDocumentType;

public interface ISalesByDocumentTypeDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByDocumentTypeReportData data);
}
