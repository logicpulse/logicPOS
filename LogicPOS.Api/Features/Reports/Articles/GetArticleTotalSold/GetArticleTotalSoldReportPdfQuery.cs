using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf
{
    public class GetArticleTotalSoldReportPdfQuery : ReportQuery
    {
        public GetArticleTotalSoldReportPdfQuery(DateTime startDate, 
                                                 DateTime endDate,
                                                 string documentType=null,
                                                 Guid? terminalId=null) : base(startDate, endDate, documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
