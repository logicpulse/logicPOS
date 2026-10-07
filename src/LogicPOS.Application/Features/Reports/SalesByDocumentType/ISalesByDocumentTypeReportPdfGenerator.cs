namespace LogicPOS.Application.Features.Reports.SalesByDocumentType;

public interface ISalesByDocumentTypeReportPdfGenerator
{
    byte[] GeneratePdf(SalesByDocumentTypeReportData data);
}
