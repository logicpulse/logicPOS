using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements
{
    public class GetStockMovementsQuery : IRequest<ErrorOr<PaginatedResult<StockMovement>>>
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ArticleId { get; set; }
        public Guid? CustomerId { get; set; }

        public string GetUrlQuery()
        {
            var query = new StringBuilder("?");

            if (StartDate.HasValue)
            {
                query.Append($"startDate={StartDate:yyyy-MM-dd}");
            }
            if (EndDate.HasValue)
            {
                query.Append($"&endDate={EndDate:yyyy-MM-dd}");
            }
            if (ArticleId.HasValue)
            {
                query.Append($"articleId={ArticleId}");
            }
            if (CustomerId.HasValue)
            {
                query.Append($"&customerId={CustomerId}");
            }
            if (Page.HasValue)
            {
                query.Append($"&page={Page}");
            }
            if (PageSize.HasValue)
            {
                query.Append($"&pageSize={PageSize}");
            }
            return query.ToString();
        }

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
    }
}
