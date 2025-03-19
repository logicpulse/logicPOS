using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits
{
    public class GetAllSizeUnitsQuery : IRequest<ErrorOr<IEnumerable<SizeUnit>>>
    {
    }
}
