using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.Periods.OpenTerminalSession
{
    public class OpenTerminalCommandHandler :
        RequestHandler<OpenTerminalSessionCommand, ErrorOr<Guid>>
    {
        public OpenTerminalCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(OpenTerminalSessionCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("worksessions/periods/open-session", command, cancellationToken);
        }
    }
}
