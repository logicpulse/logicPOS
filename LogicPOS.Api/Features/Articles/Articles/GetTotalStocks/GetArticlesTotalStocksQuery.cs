using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.GetTotalStocks
{
    public class GetArticlesTotalStocksQuery : IRequest<ErrorOr<IEnumerable<ArticleStock>>>
    {

    }
}
