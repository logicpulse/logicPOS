using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Printers.DeletePrinter
{
    public class DeletePrinterCommand : DeleteCommand
    {
        public DeletePrinterCommand(Guid id) : base(id)
        {
        }
    }
}
