using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.PoleDisplays.DeletePoleDisplay
{
    public class DeletePoleDisplayCommand : DeleteCommand
    {
        public DeletePoleDisplayCommand(Guid id) : base(id)
        {
        }
    }
}
