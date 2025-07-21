using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.CancelDocument
{
    public class CancelDocumentCommandHandler :
        RequestHandler<CancelDocumentCommand, ErrorOr<Unit>>
    {
        public CancelDocumentCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CancelDocumentCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"documents/{command.Id}/cancel", command, cancellationToken);
        }
    }
}
