using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Currencies;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.AddCurrency
{
    public class AddCurrencyCommandHandler :
        RequestHandler<AddCurrencyCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public AddCurrencyCommandHandler(IHttpClientFactory factory,  IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache= cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddCurrencyCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("currencies", command, cancellationToken);
            if(result.IsError == false)
            {
                CurrenciesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
