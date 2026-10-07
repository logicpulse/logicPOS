namespace LogicPOS.Application.Features.Reports.Stocks;

public interface IStockByWarehouseReportPdfGenerator
{
    byte[] GeneratePdf(StockReportData data);
}
