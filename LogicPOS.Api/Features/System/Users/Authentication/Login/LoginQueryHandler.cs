using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Authentication.Login
{
    public class LoginQueryHandler :
        RequestHandler<LoginQuery, ErrorOr<string>>
    {
        public LoginQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<string>> Handle(LoginQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", request, cancellationToken);

                if (response.IsSuccessStatusCode == false)
                {
                    // Prefer API ProblemDetails (e.g. invalid PIN) over a blank Unauthorized().
                    return await HandleNotSuccessfulHttpResponseAsync(response);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using (var document = JsonDocument.Parse(responseContent))
                {
                    if (document.RootElement.ValueKind == JsonValueKind.String)
                    {
                        return document.RootElement.GetString() ?? string.Empty;
                    }

                    if (document.RootElement.ValueKind == JsonValueKind.Object &&
                        document.RootElement.TryGetProperty("token", out var tokenElement))
                    {
                        return tokenElement.GetString() ?? string.Empty;
                    }

                    return Error.Unexpected("auth.invalid_response", "Resposta de autenticação inválida.");
                }
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
            catch (JsonException)
            {
                return Error.Unexpected("auth.invalid_response", "Resposta de autenticação inválida.");
            }
        }
    }
}
