using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods
{
    public class GetOpenTerminalSessionsQuery : IRequest<ErrorOr<IEnumerable<WorkSessionPeriod>>>
    {

    }
}
