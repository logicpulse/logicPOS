using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Types;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateDefaultSeries
{
    public class CreateDefaultSeriesCommandHandler : RequestHandler<CreateDefaultSeriesCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CreateDefaultSeriesCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(CreateDefaultSeriesCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandlePostCommandAsync<object>("documents/series/default", request, cancellationToken);
            if (result.IsError)
            {
                return result.Errors;
            }

            DocumentTypesCache.Clear(_keyedMemoryCache);

            return Result.Success;
        }
    }
}
