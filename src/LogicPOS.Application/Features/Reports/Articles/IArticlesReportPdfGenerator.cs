namespace LogicPOS.Application.Features.Reports.Articles;

public interface IArticlesReportPdfGenerator
{
    byte[] GeneratePdf(ArticlesReportData data);
}
