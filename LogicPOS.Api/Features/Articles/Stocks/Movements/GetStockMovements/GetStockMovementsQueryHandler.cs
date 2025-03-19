using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements
{
    public class GetStockMovementsQueryHandler :
       RequestHandler<GetStockMovementsQuery, ErrorOr<PaginatedResult<StockMovement>>>
    {
        public GetStockMovementsQueryHandler(IHttpClientFactory factory) : base(factory)
        { }

        public async override Task<ErrorOr<PaginatedResult<StockMovement>>> Handle(GetStockMovementsQuery query, CancellationToken cancellationToken = default)
        {
            var urlQuery = query.GetUrlQuery();
            return await HandleGetEntityQueryAsync<PaginatedResult<StockMovement>>($"articles/stocks/movements{urlQuery}", cancellationToken);
        }
    }
}
