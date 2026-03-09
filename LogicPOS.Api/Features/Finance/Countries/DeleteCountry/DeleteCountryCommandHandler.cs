using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Countries;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.DeleteCountry
{
    public class DeleteCountryCommandHandler :
        RequestHandler<DeleteCountryCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteCountryCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleDeleteCommandAsync($"countries/{request.Id}", cancellationToken);
            if (result.IsError == false)
            {
                CountriesCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
