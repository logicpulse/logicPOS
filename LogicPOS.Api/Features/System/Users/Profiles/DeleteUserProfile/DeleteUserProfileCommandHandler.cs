using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Profiles.DeleteUserProfile
{
    public class DeleteUserProfileCommandHandler :
        RequestHandler<DeleteUserProfileCommand, ErrorOr<bool>>
    {
        public DeleteUserProfileCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteUserProfileCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"users/profiles/{command.Id}", cancellationToken);
        }
    }
}
