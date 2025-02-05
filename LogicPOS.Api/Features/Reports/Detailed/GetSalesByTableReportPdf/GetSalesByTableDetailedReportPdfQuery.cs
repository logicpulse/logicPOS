using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByTableDetailedReportPdf
{
    public class GetSalesByTableDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByTableDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
