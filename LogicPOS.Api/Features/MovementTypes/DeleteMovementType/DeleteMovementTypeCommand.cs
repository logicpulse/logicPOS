using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.MovementTypes.DeleteMovementType
{
    public class DeleteMovementTypeCommand : DeleteCommand
    {
        public DeleteMovementTypeCommand(Guid id) : base(id)
        {
        }
    }
}
