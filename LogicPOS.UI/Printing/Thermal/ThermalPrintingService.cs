using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPrint;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing.Thermal.Printers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using static LogicPOS.UI.Printing.InvoicePrinter;
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

        public static void PrintTicket(TicketPrintingData data)
        {
            try
            {
                if (data.Items == null || data.Items.Count == 0)
                {
                    return;
                }

                // Group items by associated printer; items without association use the terminal printer.
                var itemsByPrinter = new Dictionary<Guid, List<TicketItem>>();
                var printersById = new Dictionary<Guid, Api.Entities.Printer>();
                var itemsWithoutPrinter = new List<TicketItem>();

                foreach (var item in data.Items)
                {
                    var articlePrinter = PrinterAssociationService.GetArticlePrinter(item.Id);
                    if (articlePrinter == null)
                    {
                        itemsWithoutPrinter.Add(item);
                        continue;
                    }

                    if (!itemsByPrinter.TryGetValue(articlePrinter.Id, out var printerItems))
                    {
                        printerItems = new List<TicketItem>();
                        itemsByPrinter[articlePrinter.Id] = printerItems;
                        printersById[articlePrinter.Id] = articlePrinter;
                    }

                    printerItems.Add(item);
                }

                foreach (var group in itemsByPrinter)
                {
                    PrintTicketOnPrinter(
                        new Printer(printersById[group.Key].Designation),
                        CreateTicketDataForItems(data, group.Value));
                }

                if (itemsWithoutPrinter.Count > 0 && Printer != null)
                {
                    PrintTicketOnPrinter(Printer, CreateTicketDataForItems(data, itemsWithoutPrinter));
                }
            }
            catch (Exception ex)
            {
                CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
                            .WithMessage($"Erro ao imprimir. \n\n{ex.Message}")
                            .ShowAlert();

                Log.Error(ex, "Error printing...");
            }
        }

        private static TicketPrintingData CreateTicketDataForItems(TicketPrintingData source, List<TicketItem> items)
        {
            return new TicketPrintingData
            {
                Number = source.Number,
                Table = source.Table,
                Place = source.Place,
                Items = items
            };
        }

        private static void PrintTicketOnPrinter(Printer printer, TicketPrintingData data)
        {
            new PosTicketPrinter(printer, data).Print();
        }

        public static bool PrintInvoice(InvoicePrintingData data)
        {
            try
            {
                if (Printer == null)
                {
                    return false;
                }

                new InvoicePrinter(Printer, data).Print();
                var copyNumber = data.CopyNumber > 0 ? data.CopyNumber : 1;
                DocumentsService.RegisterPrint(
                    data.DocumentId,
                    new List<int> { copyNumber },
                    data.IsSecondCopy,
                    data.Reason,
                    true);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing...");
                global::Gtk.Application.Invoke(delegate
                {
                    CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
                                .WithMessage($"Erro ao imprimir. \n\n{ex.Message}")
                                .ShowAlert();
                });
                return false;
            }
        }

        public static void PrintWorkSessionReport(DayReportData reportData)
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
                CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
                            .WithMessage($"Erro ao imprimir. \n\n{ex.Message}")
                            .ShowAlert();

                Log.Error(ex, "Error printing...");
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
                CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
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
                CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
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
                CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
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
                CustomAlerts.Error(CustomAlerts.ResolveParentWindow())
                            .WithMessage($"Erro ao imprimir. \n")
                            .ShowAlert();

                Log.Error(ex, "Error printing ticket");
            }
        }

        public static bool DocumentWasPrintedByThermalPrinter(Guid documentId)
        {
            GetDocumentPrinterTypeQuery query = new GetDocumentPrinterTypeQuery(documentId);
            var result = DependencyInjection.Mediator.Send(query).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);

            }
            return result.Value;
        }
    }
}
