using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes
{
    public class GetAllMovementTypesQuery : IRequest<ErrorOr<IEnumerable<MovementType>>>
    {

    }
}
