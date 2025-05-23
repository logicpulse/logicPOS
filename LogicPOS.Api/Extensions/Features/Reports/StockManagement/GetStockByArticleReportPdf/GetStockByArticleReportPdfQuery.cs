using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetStockByArticleReportPdf
{
    public class GetStockByArticleReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid ArticleId;

        public GetStockByArticleReportPdfQuery(DateTime startDate, DateTime endDate, Guid articleId= new Guid()) : base(startDate, endDate)
        {
            ArticleId = articleId;
        }
    }
}
