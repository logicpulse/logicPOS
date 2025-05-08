using LogicPOS.Api.Features.Common.Pagination;
using System.Text;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class GetArticlesHistoriesQuery : PaginationQuery<ArticleHistory>
    {
        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {

        }
    }
}
