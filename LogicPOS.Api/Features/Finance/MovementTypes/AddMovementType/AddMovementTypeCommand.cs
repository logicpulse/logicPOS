using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.MovementTypes.AddMovementType
{
    public class AddMovementTypeCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Notes { get; set; }
    }
}
