using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetStockByArticleGainReportPdf
{
    public class GetStockByArticleGainReportPdfQuery : ReportQuery
    {
        public Guid? ArticleId { get; set; }
        public Guid? CustomerId { get; set; }

        public GetStockByArticleGainReportPdfQuery(DateTime startDate,
                                                   DateTime endDate,
                                                   Guid? articleId,
                                                   Guid? customerId) : base(startDate, endDate, null, null)
        {
            ArticleId = articleId;
            CustomerId = customerId;
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId != null)
            {
                urlQueryBuilder.Append($"&ArticleId={ArticleId}");
            }

            if (CustomerId != null)
            {
                urlQueryBuilder.Append($"&CustomerId={CustomerId}");
            }
        }
    }
}
