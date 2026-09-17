using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
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
           return await HandleAddCommandAsync("users/profiles", command, cancellationToken);
        }
    }
}
