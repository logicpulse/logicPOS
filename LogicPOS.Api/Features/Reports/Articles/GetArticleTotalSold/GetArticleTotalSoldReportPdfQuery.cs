using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf
{
    public class GetArticleTotalSoldReportPdfQuery : ReportQuery
    {
        public GetArticleTotalSoldReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate, null, null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
