using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.POS.SalesByPlace.GetSalesByPlaceDetailedReportPdf
{
    public class GetSalesByPlaceDetailedReportPdfQuery : ReportQuery
    {
        public GetSalesByPlaceDetailedReportPdfQuery(DateTime startDate, 
                                                     DateTime endDate,
                                                     string documentType=null,
                                                     Guid? terminalId=null) : base(startDate, endDate,documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
