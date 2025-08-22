using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods
{
    public class GetOpenTerminalSessionsQueryHandler :
        RequestHandler<GetOpenTerminalSessionsQuery, ErrorOr<IEnumerable<WorkSessionPeriod>>>
    {
        public GetOpenTerminalSessionsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<WorkSessionPeriod>>> Handle(GetOpenTerminalSessionsQuery request,
                                                                                   CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<WorkSessionPeriod>($"worksessions/periods/open-terminal-sessions", cancellationToken);
        }
    }
}
