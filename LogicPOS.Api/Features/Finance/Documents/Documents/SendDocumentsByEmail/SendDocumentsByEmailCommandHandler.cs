using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.SendDocumentsByEmail
{
    public class SendDocumentsByEmailCommandHandler :
        RequestHandler<SendDocumentsByEmailCommand, ErrorOr<Success>>
    {
        public SendDocumentsByEmailCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(SendDocumentsByEmailCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleNoResponsePostCommandAsync("documents/send-by-email", command, cancellationToken);
        }
    }
}
