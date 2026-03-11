using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalDetailedReportPdf
{
    public class GetSalesByTerminalDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByTerminalDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate, null, null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
        }
    }
}
