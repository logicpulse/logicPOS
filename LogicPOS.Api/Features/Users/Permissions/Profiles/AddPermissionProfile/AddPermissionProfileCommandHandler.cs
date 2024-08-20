using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Permissions.Profiles.AddPermissionProfile
{
    public class AddPermissionProfileCommandHandler :
        RequestHandler<AddPermissionProfileCommand, ErrorOr<Guid>>
    {
        public AddPermissionProfileCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPermissionProfileCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("users/permissions/profiles",
                                                                     command,
                                                                     cancellationToken);

                return await HandleAddEntityHttpResponseAsync(httpResponse);

            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
