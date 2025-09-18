using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Receipts.CancelReceipt
{
    public class CancelReceiptCommandHandler :
        RequestHandler<CancelReceiptCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public CancelReceiptCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(CancelReceiptCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"receipts/{command.Id}/cancel", command, cancellationToken);

            if (result.IsError == false)
            {
                ReceiptsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
