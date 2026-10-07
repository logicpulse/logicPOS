namespace LogicPOS.Application.Features.Reports.Stocks;

public interface IStockByArticleGainReportPdfGenerator
{
    byte[] GeneratePdf(StockReportData data);
}
