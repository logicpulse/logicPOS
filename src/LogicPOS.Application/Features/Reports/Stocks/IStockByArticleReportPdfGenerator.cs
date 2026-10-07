namespace LogicPOS.Application.Features.Reports.Stocks;

public interface IStockByArticleReportPdfGenerator
{
    byte[] GeneratePdf(StockReportData data);
}
