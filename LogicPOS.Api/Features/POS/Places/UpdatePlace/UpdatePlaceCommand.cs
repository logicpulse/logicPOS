using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Places.UpdatePlace
{
    public class UpdatePlaceCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public Guid NewPriceTypeId { get; set; }
        public Guid NewMovementTypeId { get; set; }
        public string NewButtonImage { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
