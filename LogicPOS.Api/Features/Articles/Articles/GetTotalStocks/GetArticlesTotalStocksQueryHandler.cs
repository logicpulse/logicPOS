using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetTotalStocks
{
    public class GetArticlesTotalStocksQueryHandler :
        RequestHandler<GetArticlesTotalStocksQuery, ErrorOr<IEnumerable<ArticleStock>>>
    {
        public GetArticlesTotalStocksQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleStock>>> Handle(GetArticlesTotalStocksQuery query,
                                                                        CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<ArticleStock>("articles/stocks/totals", cancellationToken);
        }
    }
}
