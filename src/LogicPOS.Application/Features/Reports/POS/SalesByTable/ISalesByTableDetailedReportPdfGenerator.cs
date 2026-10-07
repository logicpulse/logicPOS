namespace LogicPOS.Application.Features.Reports.POS.SalesByTable;

public interface ISalesByTableDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByTableReportData data);
}