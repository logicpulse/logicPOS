using LogicPOS.Api.Features.Reports.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.GetStockMovementReportPdf
{
    public class GetStockMovementsReportPdfQuery : ReportFileQuery
    {
        public Guid? ArticleId { get; set; }
        public Guid? CustomerId { get; set; }

        public GetStockMovementsReportPdfQuery(DateTime startDate, 
                                               DateTime endDate,
                                               string documentType,
                                               Guid? terminalId
                                               ) : base(startDate, endDate, documentType, terminalId)
        {
        }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId.HasValue)
            {
                urlQueryBuilder.Append($"&ArticleId={ArticleId}");
            }

            if (CustomerId.HasValue)
            {
                urlQueryBuilder.Append($"&CustomerId={CustomerId}");
            }
        }
    }
}
