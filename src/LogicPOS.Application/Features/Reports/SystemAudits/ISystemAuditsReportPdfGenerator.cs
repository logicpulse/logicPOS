namespace LogicPOS.Application.Features.Reports.SystemAudits;

public interface ISystemAuditsReportPdfGenerator
{
    byte[] GeneratePdf(SystemAuditsReportData data);
}
