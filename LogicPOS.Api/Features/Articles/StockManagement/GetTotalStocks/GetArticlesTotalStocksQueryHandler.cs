using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks
{
    public class GetArticlesTotalStocksQueryHandler :
        RequestHandler<GetArticlesTotalStocksQuery, ErrorOr<IEnumerable<TotalStock>>>
    {
        public GetArticlesTotalStocksQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<TotalStock>>> Handle(GetArticlesTotalStocksQuery query,
                                                                        CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<TotalStock>("articles/stocks/totals", cancellationToken);
        }
    }
}
