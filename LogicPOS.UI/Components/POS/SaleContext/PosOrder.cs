using LogicPOS.Api.Entities;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.POS
{
    public class PosOrder
    {
        public Table Table { get;  }

        public List<PosTicket> Tickets { get; set; } = new List<PosTicket>();

        public PosOrder(Table table)
        {
            Table = table;
        }

        public PosTicket AddTicket(IEnumerable<SaleItem> items)
        {
            var ticket = new PosTicket((uint)Tickets.Count + 1);
            ticket.Items.AddRange(items);
            Tickets.Add(ticket);
            return ticket;
        }

    }
}
