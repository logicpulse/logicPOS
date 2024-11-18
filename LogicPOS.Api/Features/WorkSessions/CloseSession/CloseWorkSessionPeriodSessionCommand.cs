using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodSession
{
    public class CloseWorkSessionPeriodSessionCommand : IRequest<ErrorOr<Unit>>
    {
        public string Notes { get; set; }
    }
}
