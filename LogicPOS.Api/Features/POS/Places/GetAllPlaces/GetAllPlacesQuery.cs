using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Places.GetAllPlaces
{
    public class GetAllPlacesQuery : IRequest<ErrorOr<IEnumerable<Place>>>
    {
        public Guid? PlaceId { get; set; }

        public GetAllPlacesQuery(Guid? PlaceId = null) { 
            this.PlaceId = PlaceId;
        }

        public string GetUrlQuery()
        {
            return PlaceId == null ? string.Empty : $"?PlaceId={PlaceId}";
        }
    }
}
