namespace LogicPOS.Application.Features.Reports.Customers.Suppliers;

public interface ISuppliersReportPdfGenerator
{
    byte[] GeneratePdf(SuppliersReportData data);
}
