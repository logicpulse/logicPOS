using LogicPOS.Shared.Features.Company;

namespace LogicPOS.Application.Features.Reports.Common;

public abstract record ReportData
{
    public CompanyInformation Company { get; set; } = null!;
}