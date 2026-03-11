using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeReportPdf
{
    public class GetSalesByDocumentTypeReportPdfQuery : ReportQuery
    {
        public GetSalesByDocumentTypeReportPdfQuery(DateTime startDate,
                                                    DateTime endDate) : base(startDate, endDate, null, null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
