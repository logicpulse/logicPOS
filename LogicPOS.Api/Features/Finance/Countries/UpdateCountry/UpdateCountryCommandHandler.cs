using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Countries;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.UpdateCountry
{
    public class UpdateCountryCommandHandler :
        RequestHandler<UpdateCountryCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdateCountryCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateCountryCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"countries/{command.Id}", command, cancellationToken);
            if (result.IsError == false)
            {
                CountriesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
