using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetAllReportsDataDay
{
    public class GetAllReportsDataDayQueryHandler : RequestHandler<GetAllReportsDataDayQuery, ErrorOr<IEnumerable<DayReportData>>>
    {
        public GetAllReportsDataDayQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<DayReportData>>> Handle(GetAllReportsDataDayQuery request, CancellationToken ct = default)
        {
            return await HandleGetEntityQueryAsync<IEnumerable<DayReportData>>($"worksessions/days/{request.DayId}/reports", ct);
        }
    }
}
