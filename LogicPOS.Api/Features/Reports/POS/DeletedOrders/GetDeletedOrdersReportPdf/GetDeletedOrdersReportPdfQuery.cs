using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.DeletedOrders.GetDeletedOrdersReportPdf
{
    public class GetDeletedOrdersReportPdfQuery : ReportQuery
    {
        public GetDeletedOrdersReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate,null,null)
        {

        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
