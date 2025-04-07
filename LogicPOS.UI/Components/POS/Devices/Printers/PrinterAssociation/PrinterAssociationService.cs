using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Printers.PrinterAssociations.AddPrinterAssociations;
using LogicPOS.Api.Features.Printers.PrinterAssociations.GetEntityAssociatedPrinterById;
using LogicPOS.Api.Features.Printers.PrinterAssociations.RemoveEntityAssociatedPrinter;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation
{
    public static class PrinterAssociationService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public static void CreateOrRemoveAssociation(Guid entityId, Guid? printerId=null)
        {
            if (printerId.HasValue)
            {
                PrinterAssociationService.CreatePrinterAssociation((Guid)printerId, entityId);
            }
            else
            {
                PrinterAssociationService.RemovePrinterAssociation(entityId);
            }
        }
        private static void CreatePrinterAssociation(Guid printerId, Guid entityId)
        {
            var createAssociationResult = _mediator.Send(new AddPrinterAssociationCommand(printerId, entityId)).Result;
            if (createAssociationResult.IsError)
            {
                CustomAlerts.Error(POSWindow.Instance)
                            .WithMessage(createAssociationResult.FirstError.Description)
                            .ShowAlert();
                return;
            }
        }

        private static void RemovePrinterAssociation(Guid entityId)
        {
            var removeAssociationResult = _mediator.Send(new RemoveEntityAssociatedPrinterCommand(entityId)).Result;
            if (removeAssociationResult.IsError)
            {
                CustomAlerts.Error(POSWindow.Instance)
                            .WithMessage(removeAssociationResult.FirstError.Description)
                            .ShowAlert();
                return;
            }

        }

        public static Printer GetEntityAssociatedPrinterById(Guid entityId)
        {
            var entityAssociatedResult = _mediator.Send(new GetEntityAssociatedPrinterByIdQuery(entityId)).Result;
            if (entityAssociatedResult.IsError)
            {
                return null;
            }
            return entityAssociatedResult.Value;
        }
    }
}
