using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodSession
{
    public class CloseTerminalSessionCommand : IRequest<ErrorOr<Unit>>
    {
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public CloseTerminalSessionCommand(decimal amount,
                                          string notes)
        {
            Amount = amount;
            Notes = notes;
        }
    }
}
