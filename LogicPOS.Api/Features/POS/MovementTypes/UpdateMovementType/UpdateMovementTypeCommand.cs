using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.MovementTypes.UpdateMovementType
{
    public class UpdateMovementTypeCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Notes { get; set; }
        public bool VatDirectSelling { get; set; }
        public bool IsDeleted { get; set; }
    }
}
