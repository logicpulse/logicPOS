using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Stocks.Movements.GetStockMovementById
{
    public class GetStockMovementByIdQueryHandler :
         RequestHandler<GetStockMovementByIdQuery, ErrorOr<StockMovement>>
    {
        public GetStockMovementByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<StockMovement>> Handle(GetStockMovementByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<StockMovement>($"articles/stocks/movements/{query.Id}", cancellationToken);
        }
    }
}
