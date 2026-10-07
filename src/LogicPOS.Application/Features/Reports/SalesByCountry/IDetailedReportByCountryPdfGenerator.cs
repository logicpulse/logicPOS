namespace LogicPOS.Application.Features.Reports.SalesByCountry;

public interface IDetailedReportByCountryPdfGenerator
{
    byte[] GeneratePdf(SalesByCountryReportData data);
}
