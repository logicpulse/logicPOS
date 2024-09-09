using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PrinterTypes.AddPrinterType
{
    public class AddPrinterTypeCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Token { get; set; }
        public bool ThermalPrinter {  get; set; }
        public string Notes { get; set; }
    }
}
