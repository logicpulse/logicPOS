using System.Collections.Generic;

namespace LogicPOS.UI.Components.POS
{
    public class PosTicket
    {
        public uint Number { get; }
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

        public PosTicket(uint number)
        {
            Number = number;
        }
    }
}
