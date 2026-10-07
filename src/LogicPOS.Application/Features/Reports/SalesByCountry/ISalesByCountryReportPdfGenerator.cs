namespace LogicPOS.Application.Features.Reports.SalesByCountry;

public interface ISalesByCountryReportPdfGenerator
{
    byte[] GeneratePdf(SalesByCountryReportData data);
}
