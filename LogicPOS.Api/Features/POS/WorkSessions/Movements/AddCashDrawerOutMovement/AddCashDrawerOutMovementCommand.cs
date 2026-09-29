using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerOutMovement
{
    public class AddCashDrawerOutMovementCommand : IRequest<ErrorOr<Guid>>
    {
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public AddCashDrawerOutMovementCommand(decimal amount, string notes)
        {
            Amount = amount;
            Notes = notes;
        }
    }
}
