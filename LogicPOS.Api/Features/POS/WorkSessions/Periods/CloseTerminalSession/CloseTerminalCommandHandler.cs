using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodSession
{
    public class CloseTerminalCommandHandler :
        RequestHandler<CloseTerminalSessionCommand, ErrorOr<Unit>>
    {
        public CloseTerminalCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CloseTerminalSessionCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync("worksessions/periods/close-session", command, cancellationToken);
        }
    }
}
