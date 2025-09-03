using System.Collections.Generic;

namespace LogicPOS.UI.Printing.Thermal.Printers
{
    public struct TicketPrintingData
    {
        public int Number { get; set; }
        public string Table { get;  set; }
        public string Place { get;  set; }
        public List<TicketItem> Items { get; set; }
    }

    public struct TicketItem
    {
        public string Article { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}
