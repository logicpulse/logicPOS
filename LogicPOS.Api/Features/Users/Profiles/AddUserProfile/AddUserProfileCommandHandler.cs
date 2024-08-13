using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Profiles.AddUserProfile
{
    public class AddUserProfileCommandHandler : RequestHandler<AddUserProfileCommand, ErrorOr<Guid>>
    {
        public AddUserProfileCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddUserProfileCommand command, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("users/profiles",
                                                                     command,
                                                                     cancellationToken);
                var response = await httpResponse.Content.ReadAsStringAsync();

                return await HandleHttpResponseAsync(httpResponse);

            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
