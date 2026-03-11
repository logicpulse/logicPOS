using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalReportPdf
{
    public class GetSalesByTerminalReportPdfQuery : ReportQuery
    {
        public GetSalesByTerminalReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate, null,null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
        }
    }
}
