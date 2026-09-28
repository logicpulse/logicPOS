using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Authentication
{
    /// <summary>
    /// Attaches <see cref="AuthenticationData.Token"/> on every outbound request.
    /// Setting Authorization only in <c>RequestHandler</c> constructors is unreliable
    /// with <see cref="IHttpClientFactory"/> (handler constructed before login / pooled clients).
    /// </summary>
    internal sealed class BearerTokenDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = AuthenticationData.Token;

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
