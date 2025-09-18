using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.ResetPassword
{
    public class ResetPasswordCommandHandler :
        RequestHandler<ResetPasswordCommand, ErrorOr<Success>>
    {
        public ResetPasswordCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Success>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"users/{command.UserId}/reset-password", command, cancellationToken);
        }
    }
}
