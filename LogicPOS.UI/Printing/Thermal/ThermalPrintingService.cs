using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Printing.Thermal.Printers;
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
        
      

        public static void PrintTicket(TicketPrintingData data)
        {
            try
            {
                if (Printer != null)
                {
                    new PosTicketPrinter(Printer,data).Print();
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

        public static void PrintInvoice(InvoicePrintingData data)
        {
            try
            {
                if (Printer != null)
                {
                    new InvoicePrinter(Printer, data).Print();
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
