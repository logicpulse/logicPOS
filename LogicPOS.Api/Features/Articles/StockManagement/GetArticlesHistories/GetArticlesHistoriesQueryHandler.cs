using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class GetArticlesHistoriesQueryHandler :
        RequestHandler<GetArticlesHistoriesQuery, ErrorOr<IEnumerable<ArticleHistory>>>
    {
        public GetArticlesHistoriesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<ArticleHistory>>> Handle(GetArticlesHistoriesQuery query,
                                                                          CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<ArticleHistory>("articles/stocks/histories", cancellationToken);
        }
    }
}
