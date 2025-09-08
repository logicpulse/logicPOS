using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalDetailedReportPdf
{
    public class GetSalesByTerminalDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByTerminalDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
