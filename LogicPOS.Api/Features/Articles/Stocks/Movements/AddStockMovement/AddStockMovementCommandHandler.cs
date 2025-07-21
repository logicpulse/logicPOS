using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement
{
    public class AddStockMovementCommandHandler :
        RequestHandler<AddStockMovementCommand, ErrorOr<Unit>>
    {
        public AddStockMovementCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(AddStockMovementCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandlePostCommandAsync("articles/stocks/movements", command, cancellationToken);
        }
    }
}
