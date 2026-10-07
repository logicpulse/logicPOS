namespace LogicPOS.Application.Features.Reports.SalesByEmployee;

public interface ISalesByEmployeeReportPdfGenerator
{
    byte[] GeneratePdf(SalesByEmployeeReportData data);
}
