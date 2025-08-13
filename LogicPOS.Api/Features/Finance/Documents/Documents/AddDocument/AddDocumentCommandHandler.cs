using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.AddDocument
{
    public class AddDocumentCommandHandler :
        RequestHandler<AddDocumentCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public AddDocumentCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(AddDocumentCommand command, CancellationToken cancellationToken = default)
        {
            var result =  await HandleAddCommandAsync("documents",command, cancellationToken);

            if(result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
