using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Finance.Documents.Types;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateSeries
{
    public class CreateDocumentSeriesCommandHandler : 
        RequestHandler<CreateDocumentSeriesCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CreateDocumentSeriesCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(CreateDocumentSeriesCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("documents/series", command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentTypesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
