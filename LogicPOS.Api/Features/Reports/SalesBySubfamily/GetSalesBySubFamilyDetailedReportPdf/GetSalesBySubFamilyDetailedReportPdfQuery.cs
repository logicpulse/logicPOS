using LogicPOS.Api.Features.Reports.Common;
using System;

namespace LogicPOS.Api.Features.Reports.GetSalesBySubFamilyDetailedReportPdf
{
    public class GetSalesBySubFamilyDetailedReportPdfQuery : StartAndEndDateReportQuery
    {
        public GetSalesBySubFamilyDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {

        }

        public string FamilyCode { get; set; }
        public string SubfamilyCode { get; set; }
        public string ArticleCode { get; set; }
    }
}
