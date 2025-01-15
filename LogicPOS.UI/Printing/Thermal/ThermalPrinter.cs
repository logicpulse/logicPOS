using ESC_POS_USB_NET.Printer;

namespace LogicPOS.UI.Printing
{
    public abstract class ThermalPrinter
    {
        protected readonly Printer _printer;

        public ThermalPrinter(Printer printer)
        {
            _printer = printer;
        }

        public abstract void Print();
    }
}
