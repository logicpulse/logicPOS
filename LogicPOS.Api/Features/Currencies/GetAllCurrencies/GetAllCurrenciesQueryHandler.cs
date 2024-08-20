using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.GetAllCurrencies
{
    public class GetAllCurrenciesQueryHandler :
        RequestHandler<GetAllCurrenciesQuery, ErrorOr<IEnumerable<Currency>>>
    {
        public GetAllCurrenciesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<Currency>>> Handle(GetAllCurrenciesQuery request,
                                                                    CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<Currency>>("currencies",
                                                                              cancellationToken);
                return items;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
