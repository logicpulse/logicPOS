using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalReportPdf
{
    public class GetSalesByTerminalReportPdfQuery : ReportQuery
    {
        public GetSalesByTerminalReportPdfQuery(DateTime startDate, 
                                                DateTime endDate,
                                                string documentType,
                                                Guid? terminalId) : base(startDate, endDate, documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
        }
    }
}
