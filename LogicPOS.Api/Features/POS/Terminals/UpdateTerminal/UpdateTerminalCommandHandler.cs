using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.UpdateTerminal
{
    public class UpdateTerminalCommandHandler :
        RequestHandler<UpdateTerminalCommand, ErrorOr<Unit>>
    {
        public UpdateTerminalCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateTerminalCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"terminals/{command.Id}", command, cancellationToken);
        }
    }
}
