using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.SendDocumentsByEmail
{
    public class SendDocumentsByEmailCommandHandler :
        RequestHandler<SendDocumentsByEmailCommand, ErrorOr<Unit>>
    {
        public SendDocumentsByEmailCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(SendDocumentsByEmailCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("documents/send-by-email", command, cancellationToken);
                return await HandleHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
