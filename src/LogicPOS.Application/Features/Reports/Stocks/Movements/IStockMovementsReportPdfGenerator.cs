namespace LogicPOS.Application.Features.Reports.Stocks.Movements;

public interface IStockMovementsReportPdfGenerator
{
    byte[] GeneratePdf(StockMovementsReportData data);
}
