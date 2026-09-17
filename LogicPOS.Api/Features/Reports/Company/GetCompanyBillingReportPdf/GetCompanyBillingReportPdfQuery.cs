using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.Company.GetCompanyBillingReportPdf
{
    public class GetCompanyBillingReportPdfQuery : ReportFileQuery
    {
        public GetCompanyBillingReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate, null, null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
