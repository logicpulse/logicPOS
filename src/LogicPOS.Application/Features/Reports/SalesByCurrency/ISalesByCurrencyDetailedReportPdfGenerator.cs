namespace LogicPOS.Application.Features.Reports.SalesByCurrency;

public interface ISalesByCurrencyDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByCurrencyReportData data);
}
