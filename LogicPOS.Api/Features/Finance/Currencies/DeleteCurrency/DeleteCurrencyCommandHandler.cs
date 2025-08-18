using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Currencies;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.DeleteCurrency
{
    public class DeleteCurrencyCommandHandler :
        RequestHandler<DeleteCurrencyCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteCurrencyCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteCurrencyCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleDeleteCommandAsync($"currencies/{command.Id}", cancellationToken);
            if(result.IsError==false)
            {
                CurrenciesCache.Clear(_keyedMemoryCache);
            }
            return result;
        } 
    }
}
