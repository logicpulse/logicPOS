using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Places.GetAllPlaces
{
    public class GetAllPlacesQuery : IRequest<ErrorOr<IEnumerable<Place>>>
    {
    }
}
