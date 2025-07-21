using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.CloseAllSessions
{
    public class CloseAllWorkSessionsCommandHandler :
        RequestHandler<CloseAllWorkSessionsCommand, ErrorOr<Unit>>
    {
        public CloseAllWorkSessionsCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(CloseAllWorkSessionsCommand command,
                                                         CancellationToken cancellationToken = default)
        {
           return await HandleUpdateCommandAsync("worksessions/periods/close-all-sessions",
                                                 command,
                                                 cancellationToken);
        }
    }
}
