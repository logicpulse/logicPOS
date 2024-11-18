using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodSession
{
    public class CloseWorkSessionPeriodSessionCommandHandler :
        RequestHandler<CloseWorkSessionPeriodSessionCommand, ErrorOr<Unit>>
    {
        public CloseWorkSessionPeriodSessionCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CloseWorkSessionPeriodSessionCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync("worksession/periods/close-session", command, cancellationToken);
        }
    }
}
