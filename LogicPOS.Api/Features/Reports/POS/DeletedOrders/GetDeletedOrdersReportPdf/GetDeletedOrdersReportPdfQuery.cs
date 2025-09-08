using LogicPOS.Api.Features.Reports.Common;
using System;

namespace LogicPOS.Api.Features.Reports.POS.DeletedOrders.GetDeletedOrdersReportPdf
{
    public class GetDeletedOrdersReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetDeletedOrdersReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {

        }
    }
}
