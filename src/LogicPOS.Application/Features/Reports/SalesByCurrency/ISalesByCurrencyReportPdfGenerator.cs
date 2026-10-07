namespace LogicPOS.Application.Features.Reports.SalesByCurrency;

public interface ISalesByCurrencyReportPdfGenerator
{
    byte[] GeneratePdf(SalesByCurrencyReportData data);
}
