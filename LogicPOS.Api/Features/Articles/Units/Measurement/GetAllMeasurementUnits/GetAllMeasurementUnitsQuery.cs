using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits
{
    public class GetAllMeasurementUnitsQuery : IRequest<ErrorOr<IEnumerable<MeasurementUnit>>>
    {
    }
}
