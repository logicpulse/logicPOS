using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetStockByArticleGainReportPdf
{
    public class GetStockByArticleGainReportPdfQuery : StartAndEndDateReportQuery
    {
        public Guid ArticleId;
        public Guid CustomerId;

        public GetStockByArticleGainReportPdfQuery(DateTime startDate, DateTime endDate, Guid articleId = new Guid(), Guid customerId = new Guid()) : base(startDate, endDate)
        {
            ArticleId = articleId;
            CustomerId= customerId;
        }
    }
}
