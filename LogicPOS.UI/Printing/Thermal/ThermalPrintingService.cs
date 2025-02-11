using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Terminals;
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

        public static void PrintTicket(PosTicket ticket, Table table)
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new PosTicketPrinter(Printer, ticket, table).Print();
        }

        public static void PrintInvoice(Guid documentId)
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }
            new InvoicePrinter(Printer, documentId).Print();
        }

        public static void PrintWorkSessionReport(Guid workSessionId)
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new WorkSessionPrinter(Printer, workSessionId).Print();
        }
       
        public static void PrintWorkSessionDayOpen(decimal totalAmountInCashDrawer, decimal movementAmount = 0, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new CashDrawerMovementPrinter(Printer,
                                          totalAmountInCashDrawer,
                                          movementAmount,
                                          WorkSessionMovementType.CashDrawerOpen,
                                          movementDescription).Print();
        }
       
        public static void PrintCashDrawerOpen(decimal totalAmountInCashDrawer, decimal movementAmount=0, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new CashDrawerMovementPrinter(Printer,
                                          totalAmountInCashDrawer,
                                          movementAmount,
                                          WorkSessionMovementType.CashDrawerOpen,
                                          movementDescription).Print();
        }
        
        public static void PrintCashDrawerInMovement(decimal totalAmountInCashDrawer, decimal movementAmount, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new CashDrawerMovementPrinter(Printer,
                                          totalAmountInCashDrawer,
                                          movementAmount,
                                          WorkSessionMovementType.CashDrawerIn,
                                          movementDescription).Print();
        }
        
        public static void PrintCashDrawerOutMovement(decimal totalAmountInCashDrawer, decimal movementAmount, string movementDescription = "")
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new CashDrawerMovementPrinter(Printer,
                                          totalAmountInCashDrawer,
                                          movementAmount,
                                          WorkSessionMovementType.CashDrawerOut,
                                          movementDescription).Print();
        }
    }
}
