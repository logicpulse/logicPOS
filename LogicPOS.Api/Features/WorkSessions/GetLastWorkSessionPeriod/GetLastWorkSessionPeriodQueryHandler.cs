using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.GetLastWorkSessionPeriod
{
    public class GetLastWorkSessionPeriodQueryHandler :
        RequestHandler<GetLastWorkSessionPeriodQuery, ErrorOr<WorkSessionPeriod>>
    {
        public GetLastWorkSessionPeriodQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<WorkSessionPeriod>> Handle(GetLastWorkSessionPeriodQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<WorkSessionPeriod>($"worksessions/periods/last?terminalSession={query.TermnialSession}", cancellationToken);
        }
    }
}
