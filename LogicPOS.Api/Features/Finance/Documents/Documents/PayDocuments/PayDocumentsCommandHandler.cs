using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.PayDocuments
{
    public class PayDocumentsCommandHandler :
        RequestHandler<PayDocumentsCommand, ErrorOr<Guid>>
    {
        private IKeyedMemoryCache _keyedMemoryCache;

        public PayDocumentsCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(PayDocumentsCommand command, CancellationToken cancellationToken = default)
        {
            var result =  await HandleAddCommandAsync("documents/pay",command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
