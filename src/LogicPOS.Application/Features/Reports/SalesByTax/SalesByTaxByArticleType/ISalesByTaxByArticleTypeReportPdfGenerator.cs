namespace LogicPOS.Application.Features.Reports.SalesByTax.SalesByTaxByArticleType;

public interface ISalesByTaxByArticleTypeReportPdfGenerator
{
    byte[] GeneratePdf(SalesByTaxByArticleTypeReportData data);
}