using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Printers.PrinterAssociations.AddPrinterAssociations;
using LogicPOS.Api.Features.Printers.PrinterAssociations.GetEntityAssociatedPrinterById;
using LogicPOS.Api.Features.Printers.PrinterAssociations.RemoveEntityAssociatedPrinter;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using System;

namespace LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation
{
    public static class PrinterAssociationService
    {

        public static void Set(Guid entityId, Guid? printerId = null)
        {
            if (printerId.HasValue)
            {
                CreatePrinterAssociation((Guid)printerId, entityId);
                return;
            }

            RemovePrinterAssociation(entityId);
        }

        private static void CreatePrinterAssociation(Guid printerId, Guid entityId)
        {
            var createAssociationResult = DependencyInjection.Mediator.Send(new AddPrinterAssociationCommand(printerId, entityId)).Result;
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
            var removeAssociationResult = DependencyInjection.Mediator.Send(new RemoveEntityAssociatedPrinterCommand(entityId)).Result;
            if (removeAssociationResult.IsError)
            {
                CustomAlerts.Error(POSWindow.Instance)
                            .WithMessage(removeAssociationResult.FirstError.Description)
                            .ShowAlert();
                return;
            }

        }

        public static Printer GetPrinter(Guid entityId)
        {
            var entityAssociatedResult = DependencyInjection.Mediator.Send(new GetEntityAssociatedPrinterByIdQuery(entityId)).Result;
            if (entityAssociatedResult.IsError)
            {
                return null;
            }
            return entityAssociatedResult.Value;
        }
    }
}
