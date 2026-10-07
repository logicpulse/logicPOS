namespace LogicPOS.Application.Features.Reports.Customers.CurrentAccount;

public interface ICurrentAccountReportPdfGenerator
{
    byte[] GeneratePdf(CurrentAccountReportData data);
}
