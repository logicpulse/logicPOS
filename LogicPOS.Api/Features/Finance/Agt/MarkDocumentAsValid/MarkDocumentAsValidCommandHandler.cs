using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.MarkDocumentAsValid
{
    public class MarkDocumentAsValidCommandHandler : RequestHandler<MarkDocumentAsValidCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public MarkDocumentAsValidCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }


        public override async Task<ErrorOr<Success>> Handle(MarkDocumentAsValidCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"agt/fe/documents/{request.Id}/mark-as-valid", request, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
                ReceiptsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
