using ESC_POS_USB_NET.Printer;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Terminals;

namespace LogicPOS.UI.Printing
{
    public static class ThermalPrintingService
    {
        private static Printer _printer;
        public static Printer Printer
        {
            get
            {
                if (_printer == null)
                {
                    _printer = new Printer(TerminalService.Terminal.ThermalPrinter.Designation);
                }

                return _printer;
            }
        }

        public static void PrintTicket(PosTicket ticket)
        {
            if (!TerminalService.HasThermalPrinter)
            {
                return;
            }

            new PosTicketPrinter(Printer, ticket).Print();
        }
    }
}
