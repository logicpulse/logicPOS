using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.AddPrinterAssociations
{
    public class AddPrinterAssociationCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid PrinterId { get; set; }
        public Guid EntityId { get; set; }

        public AddPrinterAssociationCommand(Guid printerId, Guid entityId)
        {
            PrinterId = printerId;
            EntityId = entityId;
        }
    }
}
