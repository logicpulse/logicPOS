using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.At.RegisterDocument
{
    public class RegisterDocumentCommandHandler :
        RequestHandler<RegisterDocumentCommand, ErrorOr<RegisterDocumentResponse>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public RegisterDocumentCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<RegisterDocumentResponse>> Handle(RegisterDocumentCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandlePostCommandAsync<RegisterDocumentResponse>("at/documents/send", command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
