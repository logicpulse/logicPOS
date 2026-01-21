using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Customers.Customers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument
{
    public class IssueDocumentCommandHandler :
        RequestHandler<IssueDocumentCommand, ErrorOr<IssueDocumentResponse>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public IssueDocumentCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<IssueDocumentResponse>> Handle(IssueDocumentCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandlePostCommandAsync<IssueDocumentResponse>("documents", command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
                CustomersCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
