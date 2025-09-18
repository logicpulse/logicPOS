using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.UpdateUser
{
    public class UpdateUserCommandHandler :
        RequestHandler<UpdateUserCommand, ErrorOr<Success>>
    {
        public UpdateUserCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"users/{command.Id}", command, cancellationToken);
        }
    }
}
