using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class GetArticlesHistoriesQuery : IRequest<ErrorOr<IEnumerable<ArticleHistory>>>
    {

    }
}
