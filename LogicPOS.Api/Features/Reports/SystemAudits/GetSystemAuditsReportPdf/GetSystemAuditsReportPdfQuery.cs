using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.SystemAudits.GetSystemAuditsReportPdf
{
    public class GetSystemAuditsReportPdfQuery : ReportQuery
    {
        public GetSystemAuditsReportPdfQuery(DateTime startDate, DateTime endDate, Guid? terminalId) : base(startDate, endDate,null,terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
        }
    }
}
