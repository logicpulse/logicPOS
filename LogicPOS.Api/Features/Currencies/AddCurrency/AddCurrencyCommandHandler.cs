using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.AddCurrency
{
    public class AddCurrencyCommandHandler :
        RequestHandler<AddCurrencyCommand, ErrorOr<Guid>>
    {
        public AddCurrencyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCurrencyCommand command,
                                                                CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("currencies",
                                                                     command,
                                                                     cancellationToken);

                return await HandleAddEntityHttpResponseAsync(httpResponse);

            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
