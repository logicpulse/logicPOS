using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;
using LogicPOS.UI.Components.POS;

namespace LogicPOS.UI.Printing
{
    public class PosTicketPrinter : ThermalPrinter
    {
        private readonly PosTicket _ticket;
        public PosTicketPrinter(Printer printer, PosTicket ticket) : base(printer)
        {
            _ticket = ticket;
        }

        public override void Print()
        {
           
        }
    }
}
