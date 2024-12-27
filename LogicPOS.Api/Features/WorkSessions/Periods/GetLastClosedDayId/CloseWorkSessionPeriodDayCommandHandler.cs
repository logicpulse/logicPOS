using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.GetLastClosedDay
{
    public class GetLastClosedDayQueryHandler :
        RequestHandler<GetLastClosedDayQuery, ErrorOr<WorkSessionPeriod>>
    {
        public GetLastClosedDayQueryHandler(IHttpClientFactory factory) : base(factory)
        {

        }

        public override async Task<ErrorOr<WorkSessionPeriod>> Handle(GetLastClosedDayQuery command, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<WorkSessionPeriod>("worksessions/periods/lastday", cancellationToken);

        } 
    }
}
