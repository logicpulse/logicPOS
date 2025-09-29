using LogicPOS.Api.Features.Reports.Common;
using System;

namespace LogicPOS.Api.Features.Reports.SystemAudits.GetSystemAuditsReportPdf
{
    public class GetSystemAuditsReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSystemAuditsReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
