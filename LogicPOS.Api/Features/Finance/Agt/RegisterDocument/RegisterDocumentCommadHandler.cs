using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.RegisterDocument
{
    public class RegisterDocumentCommadHandler : RequestHandler<RegisterDocumentCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public RegisterDocumentCommadHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(RegisterDocumentCommand request, CancellationToken cancellationToken = default)
        {
            var result =  await HandleAddCommandAsync("agt/fe/documents", request, cancellationToken);
           
            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
                ReceiptsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
