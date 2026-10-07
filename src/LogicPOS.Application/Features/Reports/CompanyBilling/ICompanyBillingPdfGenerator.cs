namespace LogicPOS.Application.Features.Reports.CompanyBilling;

public interface ICompanyBillingPdfGenerator
{
    byte[] GeneratePdf(CompanyBillingReportData data);
}
