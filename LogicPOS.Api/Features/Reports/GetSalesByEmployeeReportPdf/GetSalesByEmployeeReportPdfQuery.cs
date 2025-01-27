using LogicPOS.Api.Features.Reports.Common;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByEmployeeReportPdf
{
    public class GetSalesByEmployeeReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByEmployeeReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
