using LogicPOS.Api.Features.Articles.Stocks.Common;
using LogicPOS.Api.Features.Common.Pagination;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements
{
    public class GetStockMovementsQuery : PaginationQuery<StockMovementViewModel>
    {
        public Guid? ArticleId { get; set; }
        public Guid? CustomerId { get; set; }

        public GetStockMovementsQuery GetNextPageQuery()
        {
            return new GetStockMovementsQuery
            {
                Page = (Page ?? 0) + 1,
                PageSize = PageSize,
                StartDate = StartDate,
                EndDate = EndDate,
                ArticleId = ArticleId,
                CustomerId = CustomerId
            };
        }

        public override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId.HasValue)
            {
                urlQueryBuilder.Append($"articleId={ArticleId}");
            }
            if (CustomerId.HasValue)
            {
                urlQueryBuilder.Append($"&customerId={CustomerId}");
            }
        }
    }
}
