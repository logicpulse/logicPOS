using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Profiles.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : RequestHandler<UpdateUserProfileCommand, ErrorOr<Success>>
    {
        public UpdateUserProfileCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateUserProfileCommand command,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"users/profiles/{command.Id}", command, cancellationToken);
        }
    }
}
