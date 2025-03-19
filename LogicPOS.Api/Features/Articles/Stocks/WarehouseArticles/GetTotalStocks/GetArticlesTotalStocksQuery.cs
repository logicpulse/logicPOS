using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks
{
    public class GetArticlesTotalStocksQuery : IRequest<ErrorOr<IEnumerable<TotalStock>>>
    {

    }
}
