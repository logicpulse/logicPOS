using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PrinterTypes.UpdatePrinterType
{
    public class UpdatePrinterTypeCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewToken { get; set; }
        public bool ThermalPrinter {  get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
