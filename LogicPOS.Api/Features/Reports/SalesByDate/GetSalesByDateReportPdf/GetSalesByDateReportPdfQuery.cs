using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesByDateReportPdf
{
    public class GetSalesByDateReportPdfQuery : ReportQuery
    {
        public GetSalesByDateReportPdfQuery(DateTime startDate,
                                            DateTime endDate) : base(startDate, endDate, null, null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }

}
