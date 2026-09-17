using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.Periods.TerminalIsOpen
{
    public class TerminalIsOpenQuery : IRequest<ErrorOr<bool>>
    {
        public Guid TerminalId { get; set; }

        public TerminalIsOpenQuery(Guid terminalId)
        {
            TerminalId = terminalId;
        }
    }
}
