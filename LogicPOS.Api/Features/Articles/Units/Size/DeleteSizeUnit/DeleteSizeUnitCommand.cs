using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.SizeUnits.DeleteSizeUnit
{
    public class DeleteSizeUnitCommand : DeleteCommand
    {
        public DeleteSizeUnitCommand(Guid id) : base(id)
        {
        }
    }
}
