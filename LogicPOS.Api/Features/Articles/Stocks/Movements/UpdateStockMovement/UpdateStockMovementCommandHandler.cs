using ErrorOr;
using LogicPOS.Api.Features.Articles.Articles;
using LogicPOS.Api.Features.Common.Caching;
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
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public UpdateStockMovementCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateStockMovementCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"articles/stocks/movements/{command.Id}",
                                                  command,
                                                  cancellationToken);

            if (result.IsError == false)
            {
                ArticlesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
