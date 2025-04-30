using ErrorOr;
using LogicPOS.Api.Features.Common.Pagination;
using MediatR;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class GetArticlesHistoriesQuery : IRequest<ErrorOr<PaginatedResult<ArticleHistory>>>
    {
        public string Search { get; set; }
    }
}
