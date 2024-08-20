using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Permissions.Profiles.DeletePermissionProfile
{
    public class DeletePermissionProfileCommandHandler :
        RequestHandler<DeletePermissionProfileCommand, ErrorOr<Unit>>
    {
        public DeletePermissionProfileCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(DeletePermissionProfileCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResponse = await _httpClient.DeleteAsync($"users/permissions/profiles/{request.Id}", cancellationToken);

                return await HandleDeleteEntityHttpResponseAsync(httpResponse);

            } catch(HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
