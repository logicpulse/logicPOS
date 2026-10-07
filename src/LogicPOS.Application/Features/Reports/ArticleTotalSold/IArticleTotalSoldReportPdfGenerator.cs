namespace LogicPOS.Application.Features.Reports.ArticleTotalSold;

public interface IArticleTotalSoldReportPdfGenerator
{
    byte[] GeneratePdf(ArticleTotalSoldReportData data);
}
