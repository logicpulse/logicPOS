using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesByTerminalReportPdf
{
    public class GetSalesByTerminalReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByTerminalReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
