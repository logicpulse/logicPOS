using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.GetEntityAssociatedPrinterById
{
    public class GetEntityAssociatedPrinterByIdQuery : IRequest<ErrorOr<Printer>>
    {
        public Guid Id { get; set; }

        public GetEntityAssociatedPrinterByIdQuery(Guid entityId)
        {
            Id = entityId;
        }
    }
}
