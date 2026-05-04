using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.Customers.GetCurrentAccountSummaryPdf
{
    public class GetCustomersCurrentAccountSummaryReportPdfQuery : ReportFileQuery
    {
        public GetCustomersCurrentAccountSummaryReportPdfQuery(DateTime startDate, DateTime endDate, Guid? customerId = null) : base(
            startDate, endDate, null, null)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; set; }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (CustomerId.HasValue)
            {
                urlQueryBuilder.Append($"&CustomerId={CustomerId}");
            }
        }
    }
}
