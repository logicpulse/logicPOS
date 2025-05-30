using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.DiscountGroups.DeleteDiscountGroup
{
    public class DeleteDiscountGroupCommand : DeleteCommand
    {
        public DeleteDiscountGroupCommand(Guid id) : base(id)
        {
        }
    }
}
