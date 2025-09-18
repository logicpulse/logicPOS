using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Printers.UpdatePrinter
{
    public class UpdatePrinterCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public Guid NewTypeId { get; set; }
        public string NewNetworkName { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
