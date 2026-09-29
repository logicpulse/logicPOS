using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PrinterTypes.UpdatePrinterType
{
    public class UpdatePrinterTypeCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Token { get; set; }
        public bool ThermalPrinter {  get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
