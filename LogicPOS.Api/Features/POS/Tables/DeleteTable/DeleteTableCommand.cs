using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Tables.DeleteTable
{
    public class DeleteTableCommand : DeleteCommand
    {
        public DeleteTableCommand(Guid id) : base(id)
        {
        }
    }
}
