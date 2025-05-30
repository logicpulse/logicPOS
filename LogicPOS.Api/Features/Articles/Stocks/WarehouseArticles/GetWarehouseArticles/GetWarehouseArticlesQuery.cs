using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common;
using LogicPOS.Api.Features.Common.Pagination;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetWarehouseArticles
{
    public class GetWarehouseArticlesQuery : PaginationQuery<WarehouseArticleViewModel>
    {
        public Guid? ArticleId { get; set; }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId.HasValue)
            {
                urlQueryBuilder.Append($"articleId={ArticleId}");
            }
        }
    }
}
