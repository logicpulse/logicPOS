using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByEmployeeDetailedReportPdf
{
    public class GetSalesByEmployeeDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByEmployeeDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
