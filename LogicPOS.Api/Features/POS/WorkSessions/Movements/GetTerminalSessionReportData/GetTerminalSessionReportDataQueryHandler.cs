using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetTerminalSessionReportData
{
    public class GetTerminalSessionReportDataQueryHandler : RequestHandler<GetTerminalSessionReportDataQuery, ErrorOr<DayReportData>>
    {
        public GetTerminalSessionReportDataQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<DayReportData>> Handle(GetTerminalSessionReportDataQuery request, CancellationToken ct = default)
        {
            return await HandleGetQueryAsync<DayReportData>($"worksessions/terminal-sessions/{request.TerminalSessionId}/report", ct);
        }
    }
}
