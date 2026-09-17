using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Printers.AddPrinter
{
    public class AddPrinterCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public Guid TypeId { get; set; }
        public string NetworkName { get; set; }
        public string Notes { get; set; }
    }
}
