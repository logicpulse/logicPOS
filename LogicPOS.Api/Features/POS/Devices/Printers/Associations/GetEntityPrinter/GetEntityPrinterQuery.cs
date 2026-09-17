using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Printers.PrinterAssociations.GetEntityAssociatedPrinterById
{
    public class GetEntityPrinterQuery : IRequest<ErrorOr<Printer>>
    {
        public Guid Id { get; set; }

        public GetEntityPrinterQuery(Guid entityId)
        {
            Id = entityId;
        }
    }
}
