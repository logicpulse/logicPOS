using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Profiles.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : RequestHandler<UpdateUserProfileCommand, ErrorOr<Unit>>
    {
        public UpdateUserProfileCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateUserProfileCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"users/profiles/{command.Id}", command, cancellationToken);
                return await HandleUpdateEntityHttpResponseAsync(response);
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
