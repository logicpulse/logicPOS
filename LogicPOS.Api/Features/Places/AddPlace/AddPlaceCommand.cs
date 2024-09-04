using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Places.AddPlace
{
    public class AddPlaceCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string ButtonImage { get; set; }
        public Guid PriceTypeId { get; set; }
        public Guid MovementTypeId { get; set; }
    }
}
