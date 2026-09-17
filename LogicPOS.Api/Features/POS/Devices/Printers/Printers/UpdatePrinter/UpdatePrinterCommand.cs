using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Printers.UpdatePrinter
{
    public class UpdatePrinterCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid TypeId { get; set; }
        public string NetworkName { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
