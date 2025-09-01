using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.RemoveEntityAssociatedPrinter
{
    public class RemoveEntityAssociatedPrinterCommand : DeleteCommand
    {
        public RemoveEntityAssociatedPrinterCommand(Guid entityId) : base(entityId)
        {
        }
    }
}
