namespace LogicPOS.Application.Features.Reports.SalesByTax.SalesByTaxGroup;

public interface ISalesByTaxGroupDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByTaxReportData data);
}
