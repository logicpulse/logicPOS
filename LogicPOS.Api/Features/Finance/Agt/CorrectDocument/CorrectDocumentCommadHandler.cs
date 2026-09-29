using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.CorrectDocument
{
    public class CorrectDocumentCommadHandler : RequestHandler<CorrectDocumentCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CorrectDocumentCommadHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(CorrectDocumentCommand request, CancellationToken cancellationToken = default)
        {
            var result =  await HandleAddCommandAsync("agt/fe/documents/correct", request, cancellationToken);
           
            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
                ReceiptsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
