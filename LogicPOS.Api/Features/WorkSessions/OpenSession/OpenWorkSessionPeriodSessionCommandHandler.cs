using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodSession
{
    public class OpenWorkSessionPeriodSessionCommandHandler :
        RequestHandler<OpenWorkSessionPeriodSessionCommand, ErrorOr<Guid>>
    {
        public OpenWorkSessionPeriodSessionCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(OpenWorkSessionPeriodSessionCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("worksession/periods/open-session", command, cancellationToken);
        }
    }
}
