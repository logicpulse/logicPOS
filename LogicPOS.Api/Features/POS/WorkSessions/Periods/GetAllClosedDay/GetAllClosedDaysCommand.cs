using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.WorkSessions.GetAllClosedDays

{
    public class GetAllClosedDaysQuery : IRequest<ErrorOr<IEnumerable<WorkSessionPeriod>>>
    {
       
    }
}
