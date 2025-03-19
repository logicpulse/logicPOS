using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.MeasurementUnits.DeleteMeasurementUnit
{
    public class DeleteMeasurementUnitCommand : DeleteCommand
    {
        public DeleteMeasurementUnitCommand(Guid id) : base(id)
        { }
    }
}
