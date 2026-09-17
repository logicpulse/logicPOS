using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Users.CommissionGroups.DeleteCommissionGroup
{
    public class DeleteCommissionGroupCommand : DeleteCommand
    {
        public DeleteCommissionGroupCommand(Guid id) : base(id)
        {
        }
    }
}
