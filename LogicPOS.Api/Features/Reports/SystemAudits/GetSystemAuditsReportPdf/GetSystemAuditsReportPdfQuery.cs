using LogicPOS.Api.Features.Reports.Common;
using System;

namespace LogicPOS.Api.Features.Reports.SystemAudits.GetSystemAuditsReportPdf
{
    public class GetSystemAuditsReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid TerminalId { get; set; }
        public GetSystemAuditsReportPdfQuery(DateTime startDate, DateTime endDate, Guid terminalId) : base(startDate, endDate)
        {
            TerminalId = terminalId;
        }
    }
}
