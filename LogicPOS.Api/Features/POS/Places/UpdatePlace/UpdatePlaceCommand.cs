using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Places.UpdatePlace
{
    public class UpdatePlaceCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid PriceTypeId { get; set; }
        public Guid MovementTypeId { get; set; }
        public string ButtonImage { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
