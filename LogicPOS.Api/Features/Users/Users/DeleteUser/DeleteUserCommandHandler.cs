using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.DeleteUser
{
    public class DeleteUserCommandHandler :
        RequestHandler<DeleteUserCommand, ErrorOr<bool>>
    {
        public DeleteUserCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteUserCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"users/details/{command.Id}", cancellationToken);
        }
    }
}
