using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.UI.Components.Terminals;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Printer = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    public static class ThermalPrintingService
    {
        private static Printer _printer;
        public static Printer Printer
        {
            get
            {
                if (_printer == null && TerminalService.HasThermalPrinter)
                {
                    _printer = new Printer(TerminalService.Terminal.ThermalPrinter.Designation);
                }

                return _printer;
            }
        }
        
        public static void GetEntityAssociatedPrinter(Guid? documentId = null, PosTicket ticket = null, bool isDocument = false)
        {
            Document document = null;
            Api.Entities.Printer printer = null;


            if (isDocument && documentId.HasValue)
            {
                GetPrintFromAssociatedDocumentDetail((Guid)documentId, ref document, ref printer);
            }
            else
            {
                GetPrinterFromAssociatedTicketItem(ticket, ref printer);
            }
            if (printer != null && printer.Type.ThermalPrinter)
            {
                _printer = new Printer(printer.Designation);
            }
        }

        private static void GetPrinterFromAssociatedTicketItem(PosTicket ticket, ref Api.Entities.Printer printer)
        {
            if (ticket == null)
            {
                return;
            }
            if (ticket != null && ticket.Items.Count != 1)
            {
                return;
            }

            printer = PrinterAssociationService.GetEntityAssociatedPrinterById(ticket.Items[0].Article.Id);
            if (printer == null)
            {
                printer = PrinterAssociationService.GetEntityAssociatedPrinterById(ticket.Items[0].Article.SubfamilyId);
            }
            if (printer == null)
            {
                printer = PrinterAssociationService.GetEntityAssociatedPrinterById(ticket.Items[0].Article.FamilyId);
            }

            if (printer == null && !TerminalService.HasThermalPrinter)
            {
                return;
            }
        }

        private static void GetPrintFromAssociatedDocumentDetail(Guid documentId, ref Document document, ref Api.Entities.Printer printer)
        {
            var result = DependencyInjection.Mediator.Send(new GetDocumentByIdQuery(documentId)).Result;
            if (result.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(result.FirstError.Description)
                            .ShowAlert();
            }

            document = result.Value;

            if (document != null && document.Details.Count != 1)
            {
                return;
            }

            printer = PrinterAssociationService.GetEntityAssociatedPrinterById(document.Details[0].ArticleId);
            if (printer == null)
            {
                printer = PrinterAssociationService.GetEntityAssociatedPrinterById(document.Details[0].Article.SubfamilyId);
            }
            if (printer == null)
            {
                printer = PrinterAssociationService.GetEntityAssociatedPrinterById(document.Details[0].Article.Subfamily.FamilyId);
            }

            if (printer == null && !TerminalService.HasThermalPrinter)
            {
                return;

            }

            if (printer != null && !printer.Type.ThermalPrinter)
            {
                var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(documentId, 1);
                PdfPrinter.Print(tempFile.Value.Path, printer.Designation);
            }

        }

        public static void PrintTicket(PosTicket ticket, Table table)
        {
            try
            {
                GetEntityAssociatedPrinter(null, ticket);
                if (Printer != null)
                {
                    new PosTicketPrinter(Printer, ticket, table).Print();
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }

        public static void PrintInvoice(Guid documentId)
        {
            try
            {
                GetEntityAssociatedPrinter(documentId, null, true);
                if (Printer != null)
                {
                    new InvoicePrinter(Printer, documentId).Print();
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }

        public static void PrintWorkSessionReport(WorkSessionData reportData)
        {
            if (Printer == null || reportData == null)
            {
                return;
            }

            try
            {
                new WorkSessionPrinter(Printer, reportData).Print();
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }
 
        public static void PrintCashDrawerOpen(decimal totalAmountInCashDrawer, decimal movementAmount = 0, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            try
            {
                if (Printer != null)
                {
                    new CashDrawerMovementPrinter(Printer,
                                                 totalAmountInCashDrawer,
                                                 movementAmount,
                                                 WorkSessionMovementType.CashDrawerOpen,
                                                 movementDescription).Print();
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }

        public static void PrintCashDrawerClose(decimal totalAmountInCashDrawer, decimal movementAmount = 0, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            try
            {
                if (Printer != null)
                {
                    new CashDrawerMovementPrinter(Printer,
                                                 totalAmountInCashDrawer,
                                                 movementAmount,
                                                 WorkSessionMovementType.CashDrawerClose,
                                                 movementDescription).Print();
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }

        public static void PrintCashDrawerInMovement(decimal totalAmountInCashDrawer, decimal movementAmount, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            try
            {
                new CashDrawerMovementPrinter(Printer,
                                              totalAmountInCashDrawer,
                                              movementAmount,
                                              WorkSessionMovementType.CashDrawerIn,
                                              movementDescription).Print();
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }

        public static void PrintCashDrawerOutMovement(decimal totalAmountInCashDrawer, decimal movementAmount, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            try
            {
                if (Printer != null)
                {
                    new CashDrawerMovementPrinter(Printer,
                                              totalAmountInCashDrawer,
                                              movementAmount,
                                              WorkSessionMovementType.CashDrawerOut,
                                              movementDescription).Print();
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.Error()
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }
    }
}
