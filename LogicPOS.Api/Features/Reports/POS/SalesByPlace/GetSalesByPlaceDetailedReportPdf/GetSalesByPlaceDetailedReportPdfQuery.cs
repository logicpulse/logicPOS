using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByPlace.GetSalesByPlaceDetailedReportPdf
{
    public class GetSalesByPlaceDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByPlaceDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate,null,null)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
