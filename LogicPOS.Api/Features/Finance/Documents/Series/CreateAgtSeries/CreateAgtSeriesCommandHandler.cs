using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Finance.Documents.Types;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateAgtSeries
{
    public class CreateAgtSeriesCommandHandler : 
        RequestHandler<CreateAgtSeriesCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CreateAgtSeriesCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(CreateAgtSeriesCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("documents/agt-series", command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentTypesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
