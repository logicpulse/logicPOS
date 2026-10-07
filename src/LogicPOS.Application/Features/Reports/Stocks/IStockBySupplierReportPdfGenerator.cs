namespace LogicPOS.Application.Features.Reports.Stocks;

public interface IStockBySupplierReportPdfGenerator
{
    byte[] GeneratePdf(StockReportData data);
}
