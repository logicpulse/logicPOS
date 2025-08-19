using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions
{
    public class GetLastWorkSessionByTerminalIdQuery : IRequest<ErrorOr<WorkSessionPeriod>>
    {
        public Guid TerminalId { get; set; }

        public GetLastWorkSessionByTerminalIdQuery(Guid terminalId)
        {
            TerminalId = terminalId;
        }
    }
}
