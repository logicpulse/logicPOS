using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.AddStock
{
    public class AddArticlesStocksCommandHandler :
        RequestHandler<AddArticlesStockCommand, ErrorOr<Unit>>
    {
        public AddArticlesStocksCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(AddArticlesStockCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("articles/stocks", command, cancellationToken);
                return await HandleHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
