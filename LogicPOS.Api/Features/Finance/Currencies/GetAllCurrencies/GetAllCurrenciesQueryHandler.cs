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
            return await HandleGetEntitiesQueryAsync<Currency>("currencies",cancellationToken);
        }
    }
}
