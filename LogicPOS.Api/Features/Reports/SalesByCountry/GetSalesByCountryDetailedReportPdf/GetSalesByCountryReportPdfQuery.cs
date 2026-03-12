using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByCountryDetailedReportPdf
{
    public class GetSalesByCountryDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByCountryDetailedReportPdfQuery(DateTime startDate, 
                                                       DateTime endDate,
                                                       string documentType=null,
                                                       Guid? terminalId = null) : base(startDate, endDate, documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
