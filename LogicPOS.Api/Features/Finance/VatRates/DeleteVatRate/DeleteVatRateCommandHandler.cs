using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.VatRates;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.DeleteVatRate
{
    public class DeleteVatRateCommandHandler :
        RequestHandler<DeleteVatRateCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteVatRateCommandHandler(IHttpClientFactory factory,  IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteVatRateCommand commmand, CancellationToken cancellationToken = default)
        {
            var result= await HandleDeleteCommandAsync($"vatrates/{commmand.Id}", cancellationToken);
            if (result.IsError == false)
            {
                VatRatesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
