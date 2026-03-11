using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetSalesBySubFamilyDetailedReportPdf
{
    public class GetSalesBySubFamilyDetailedReportPdfQuery : ReportQuery
    {
        public string FamilyCode { get; set; }
        public string SubfamilyCode { get; set; }
        public string ArticleCode { get; set; }

        public GetSalesBySubFamilyDetailedReportPdfQuery(DateTime startDate, DateTime endDate) : base(startDate, endDate, null, null)
        {

        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (!string.IsNullOrEmpty(FamilyCode))
            {
                urlQueryBuilder.Append($"&FamilyCode={FamilyCode}");
            }
            if (!string.IsNullOrEmpty(SubfamilyCode))
            {
                urlQueryBuilder.Append($"&SubfamilyCode={SubfamilyCode}");
            }
            if (!string.IsNullOrEmpty(ArticleCode))
            {
                urlQueryBuilder.Append($"&ArticleCode={ArticleCode}");
            }
        }
    }
}
