using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods
{
    public class GetAllWorkSessionPeriodsQuery : IRequest<ErrorOr<IEnumerable<WorkSessionPeriod>>>
    {
        public WorkSessionPeriodType Type { get; set; }
        public WorkSessionPeriodStatus Status { get; set; }

        public GetAllWorkSessionPeriodsQuery(WorkSessionPeriodType type, WorkSessionPeriodStatus status)
        {
            Type = type;
            Status = status;
        }
    }
}
