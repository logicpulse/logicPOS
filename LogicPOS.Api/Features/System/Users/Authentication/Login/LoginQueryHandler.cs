using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
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
                    return Error.Unauthorized();
                }

                return (await response.Content.ReadAsStringAsync()).Trim('"');
            }
            catch (HttpRequestException)
            {
                return ApiErrors.APICommunication;
            }
        }
    }
}
