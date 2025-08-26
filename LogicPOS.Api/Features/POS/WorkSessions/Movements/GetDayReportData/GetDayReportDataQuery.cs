using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData
{
    public class GetDayReportDataQuery : IRequest<ErrorOr<DayReportData>>
    {
        public Guid DayId { get; set; }

        public GetDayReportDataQuery(Guid dayId)
        {
            DayId = dayId;
        }
    }
}
