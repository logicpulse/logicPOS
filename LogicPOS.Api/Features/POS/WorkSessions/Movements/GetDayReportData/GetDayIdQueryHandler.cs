using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData
{
    public class GetDayIdQueryHandler : RequestHandler<GetDayReportDataQuery, ErrorOr<DayReportData>>
    {
        public GetDayIdQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<DayReportData>> Handle(GetDayReportDataQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<DayReportData>($"worksessions/days/{request.DayId}/report", ct);
        }
    }
}
