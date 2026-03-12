using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByCustomerReportPdf
{
    public class GetSalesByCustomerReportPdfQuery : ReportQuery
    {
        public GetSalesByCustomerReportPdfQuery(DateTime startDate, 
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
