using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SystemAudits;

public record SystemAuditsReportData : ReportData
{
    public List<Audit> Audits { get; set; } = null!;
}

public record Audit
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public string UserName { get; set; }=default!;
    public string TerminalDesignation { get; set; }=null!;
}

