using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Finance.Documents.Types;

namespace LogicPOS.Api.Features.Documents.Series.AddDocumentSerie
{
    public class AddDocumentSerieCommandHandler : 
        RequestHandler<AddDocumentSerieCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public AddDocumentSerieCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddDocumentSerieCommand command, CancellationToken cancellationToken = default)
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
