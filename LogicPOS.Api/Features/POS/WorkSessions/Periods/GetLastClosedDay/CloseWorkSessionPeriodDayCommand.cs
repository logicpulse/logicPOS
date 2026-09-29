using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.GetLastClosedDay
{
    public class GetLastClosedDayQuery : IRequest<ErrorOr<WorkSessionPeriod>>
    {
       
    }
}
