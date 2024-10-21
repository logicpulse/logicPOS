using LogicPOS.Api.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public class PosOrder
    {
        public Table Table { get;  }

        public List<PosTicket> Tickets { get; } = new List<PosTicket>();

        public PosOrder(Table table)
        {
            Table = table;
        }

        public List<SaleItem> GetOrderItems()
        {
            var orderItems = new List<SaleItem>();

            var ticketsItems = Tickets.SelectMany(t => t.Items);

            foreach (var item in ticketsItems)
            {
                var existingItem = orderItems.FirstOrDefault(i => i.Article.Id == item.Article.Id);

                if (existingItem == null)
                {
                    orderItems.Add(new SaleItem(item.Article)
                    {
                        Quantity = item.Quantity
                    });
                }
                else
                {
                    existingItem.Quantity += item.Quantity;
                }
            }

            return orderItems; 
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
