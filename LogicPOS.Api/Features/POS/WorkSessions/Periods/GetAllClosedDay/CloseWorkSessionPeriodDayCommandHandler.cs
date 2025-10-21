using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.GetAllClosedDays
{
    public class GetAllClosedDaysQueryHandler :
        RequestHandler<GetAllClosedDaysQuery, ErrorOr<IEnumerable<WorkSessionPeriod>>>
    {
        public GetAllClosedDaysQueryHandler(IHttpClientFactory factory) : base(factory)
        {

        }

        public override async Task<ErrorOr<IEnumerable<WorkSessionPeriod>>> Handle(GetAllClosedDaysQuery command, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<IEnumerable<WorkSessionPeriod>>("worksessions/periods/alldays", cancellationToken);

        }
    }
}
