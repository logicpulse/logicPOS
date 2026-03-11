using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetStockByArticleReportPdf
{
    public class GetStockByArticleReportPdfQuery : ReportQuery
    {
        public Guid? ArticleId;

        public GetStockByArticleReportPdfQuery(DateTime startDate, DateTime endDate, Guid articleId) : base(startDate, endDate, null, null)
        {
            ArticleId = articleId;
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId.HasValue)
            {
                urlQueryBuilder.Append($"&ArticleId={ArticleId}");
            }
        }
    }
}
