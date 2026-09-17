using ErrorOr;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetAllReportsDataDay
{
    public class GetAllReportsDataDayQuery : IRequest<ErrorOr<IEnumerable<DayReportData>>>
    {
        public Guid DayId { get; set; }

        public GetAllReportsDataDayQuery(Guid dayId)
        {
            DayId = dayId;
        }
    }
}
