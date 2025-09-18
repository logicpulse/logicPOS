using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.UpdateStockMovement
{
    public class UpdateStockMovementCommandHandler :
        RequestHandler<UpdateStockMovementCommand, ErrorOr<Success>>
    {
        public UpdateStockMovementCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateStockMovementCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/stocks/movements/{command.Id}",
                                                  command,
                                                  cancellationToken);
        }
    }
}
