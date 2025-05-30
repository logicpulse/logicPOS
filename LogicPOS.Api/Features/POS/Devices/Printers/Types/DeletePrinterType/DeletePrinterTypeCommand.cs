using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.PrinterTypes.DeletePrinterType
{
    public class DeletePrinterTypeCommand : DeleteCommand
    {
        public DeletePrinterTypeCommand(Guid id) : base(id)
        {
        }
    }
}
