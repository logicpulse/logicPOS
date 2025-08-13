using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.CancelDocument
{
    public class CancelDocumentCommandHandler :
        RequestHandler<CancelDocumentCommand, ErrorOr<Unit>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CancelDocumentCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Unit>> Handle(CancelDocumentCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            var result =  await HandleUpdateCommandAsync($"documents/{command.Id}/cancel", command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
