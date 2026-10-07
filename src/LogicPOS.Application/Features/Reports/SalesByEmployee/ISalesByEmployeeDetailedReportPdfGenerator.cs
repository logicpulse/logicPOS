namespace LogicPOS.Application.Features.Reports.SalesByEmployee;

public interface ISalesByEmployeeDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByEmployeeReportData data);
}
