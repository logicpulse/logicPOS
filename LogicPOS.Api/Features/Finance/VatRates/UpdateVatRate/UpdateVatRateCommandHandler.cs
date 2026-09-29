using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.VatRates;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.UpdateVatRate
{
    public class UpdateVatRateCommandHandler :
        RequestHandler<UpdateVatRateCommand, ErrorOr<Success>>
    {
        private IKeyedMemoryCache _keyedMemoryCache;
        public UpdateVatRateCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateVatRateCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"vatrates/{command.Id}", command, cancellationToken);
            if (result.IsError == false)
            {
                VatRatesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
