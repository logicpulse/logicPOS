using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Customers.GetCurrentAccountPdf
{
    public class GetCustomerCurrentAccountPdfQuery : ReportFileQuery
    {
        public GetCustomerCurrentAccountPdfQuery(DateTime startDate,
            DateTime endDate, Guid customerId) : base(startDate, endDate, null, null)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; set; }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            urlQueryBuilder.Append($"&CustomerId={CustomerId}");
        }
    }
}
