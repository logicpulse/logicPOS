using ErrorOr;
using LogicPOS.Api.Features.Articles.Stocks.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements
{
    public class GetStockMovementsQueryHandler :
       RequestHandler<GetStockMovementsQuery, ErrorOr<PaginatedResult<StockMovementViewModel>>>
    {
        public GetStockMovementsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<PaginatedResult<StockMovementViewModel>>> Handle(GetStockMovementsQuery query, CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetQueryAsync<PaginatedResult<StockMovementViewModel>>($"articles/stocks/movements{urlQuery}", cancellationToken);
        }
    }
}
