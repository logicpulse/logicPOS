using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.WeighingMachines.GetAllWeighingMachines
{
    public class GetAllWeighingMachinesQuery : IRequest<ErrorOr<IEnumerable<WeighingMachine>>>
    {
    }
}
