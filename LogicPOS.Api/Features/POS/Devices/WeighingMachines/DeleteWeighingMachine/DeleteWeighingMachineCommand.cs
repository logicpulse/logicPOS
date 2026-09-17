using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.WeighingMachines.DeleteWeighingMachine
{
    public class DeleteWeighingMachineCommand : DeleteCommand
    {
        public DeleteWeighingMachineCommand(Guid id) : base(id)
        {
        }
    }
}
