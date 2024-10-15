using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Receipts.CancelReceipt
{
    public class CancelReceiptCommandHandler :
        RequestHandler<CancelReceiptCommand, ErrorOr<Unit>>
    {
        public CancelReceiptCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CancelReceiptCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"receipts/{command.Id}/cancel", command, cancellationToken);
        }
    }
}
