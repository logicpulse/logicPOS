using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Currencies;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.UpdateCurrency
{
    public class UpdateCurrencyCommandHandler : RequestHandler<UpdateCurrencyCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdateCurrencyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateCurrencyCommand command,
                                                           CancellationToken cancellationToken = default)
        {
           var result= await HandleUpdateCommandAsync($"currencies/{command.Id}", command, cancellationToken);
            if (result.IsError == false)
            {
                CurrenciesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
