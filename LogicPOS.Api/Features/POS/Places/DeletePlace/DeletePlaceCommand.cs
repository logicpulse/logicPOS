using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Places.DeletePlace
{
    public class DeletePlaceCommand : DeleteCommand
    {
        public DeletePlaceCommand(Guid id) : base(id)
        {
        }
    }
}
