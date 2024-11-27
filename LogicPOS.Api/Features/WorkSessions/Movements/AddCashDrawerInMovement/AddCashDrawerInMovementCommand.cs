using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerInMovement
{
    public class AddCashDrawerInMovementCommand : IRequest<ErrorOr<Guid>>
    {
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public AddCashDrawerInMovementCommand(decimal amount, string notes)
        {
            Amount = amount;
            Notes = notes;
        }
    }
}
