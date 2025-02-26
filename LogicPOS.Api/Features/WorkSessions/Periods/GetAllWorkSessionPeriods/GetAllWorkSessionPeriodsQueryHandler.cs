using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods
{
    public class GetAllWorkSessionPeriodsQueryHandler :
        RequestHandler<GetAllWorkSessionPeriodsQuery, ErrorOr<IEnumerable<WorkSessionPeriod>>>
    {
        public GetAllWorkSessionPeriodsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<WorkSessionPeriod>>> Handle(GetAllWorkSessionPeriodsQuery request,
                                                                                   CancellationToken cancellationToken = default)
        {
            int status = (int)request.Status;
            int type = (int)request.Type;
            return await HandleGetEntitiesQueryAsync<WorkSessionPeriod>($"worksessions/periods?status={status}&type={type}", cancellationToken);
        }
    }
}
