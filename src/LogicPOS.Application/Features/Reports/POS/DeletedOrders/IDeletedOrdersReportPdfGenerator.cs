namespace LogicPOS.Application.Features.Reports.POS.DeletedOrders;

public interface IDeletedOrdersReportPdfGenerator
{
    byte[] GeneratePdf(DeletedOrdersReportData data);
}