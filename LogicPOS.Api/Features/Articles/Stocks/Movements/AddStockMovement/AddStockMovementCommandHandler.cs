using ErrorOr;
using LogicPOS.Api.Features.Articles.Articles;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement
{
    public class AddStockMovementCommandHandler :
        RequestHandler<AddStockMovementCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public AddStockMovementCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(AddStockMovementCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            var result = await HandleNoResponsePostCommandAsync("articles/stocks/movements", command, cancellationToken);

            if (result.IsError == false)
            {
                ArticlesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
