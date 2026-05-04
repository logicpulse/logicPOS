using ErrorOr;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using MediatR;
using System;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetTerminalSessionReportData
{
    public class GetTerminalSessionReportDataQuery : IRequest<ErrorOr<DayReportData>>
    {
        public Guid TerminalSessionId { get; set; }

        public GetTerminalSessionReportDataQuery(Guid terminalSessionId)
        {
            TerminalSessionId = terminalSessionId;
        }
    }
}
