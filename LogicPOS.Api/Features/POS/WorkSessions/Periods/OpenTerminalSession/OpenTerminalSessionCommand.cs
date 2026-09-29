using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.Periods.OpenTerminalSession
{
    public class OpenTerminalSessionCommand : IRequest<ErrorOr<Guid>>
    {
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public OpenTerminalSessionCommand(decimal amount,
                                          string notes)
        {
            Amount = amount;
            Notes = notes;
        }
    }
}
