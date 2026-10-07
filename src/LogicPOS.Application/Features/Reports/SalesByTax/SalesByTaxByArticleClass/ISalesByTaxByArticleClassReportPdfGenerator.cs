namespace LogicPOS.Application.Features.Reports.SalesByTax.SalesByTaxByArticleClass;

public interface ISalesByTaxByArticleClassReportPdfGenerator
{
    byte[] GeneratePdf(SalesByTaxByArticleClassReportData data);
}
