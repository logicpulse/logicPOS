using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
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

        public override async Task<ErrorOr<Unit>> Handle(
            DeletePermissionProfileCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"users/permissions/profiles/{command.Id}", cancellationToken);
        }
    }
}
