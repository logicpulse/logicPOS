using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalReportPdf
{
    public class GetSalesByTerminalReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesByTerminalReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
}
