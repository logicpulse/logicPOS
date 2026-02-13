using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class GetArticlesHistoriesQuery : PaginationQuery<ArticleHistory>
    {
        public Guid ArticleId { get; set; }
        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (ArticleId!=Guid.Empty)
            {
                urlQueryBuilder.Append($"&articleId={ArticleId}");
            }
        }
    }
}
