using LogicPOS.Api.Entities;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDetailDto = LogicPOS.Api.Features.Documents.AddDocument.DocumentDetail;


namespace LogicPOS.UI.Components.POS
{
    public class PosOrder
    {
        public Guid? Id { get; set; } = null;
        public Table Table { get; }

        public List<PosTicket> Tickets { get; } = new List<PosTicket>();

        public PosOrder(Table table)
        {
            Table = table;
        }

        public PosOrder(Order order)
        {
            Id = order.Id;
            Table = order.Table;
            foreach (var ticket in order.Tickets)
            {
                AddTicket(ticket);
            }
        }

        public void SplitTickets(int splitNumber)
        {
            if (Tickets.Any())
            {

                foreach (var ticket in Tickets)
                {
                    foreach (var item in ticket.Items)
                    {
                        item.Quantity -= (item.Quantity / splitNumber);
                    }
                }
            }
        }
        public List<SaleItem> GetOrderItems()
        {
            var orderItems = new List<SaleItem>();

            var ticketsItems = Tickets.SelectMany(t => t.Items);

            return SaleItem.Compact(ticketsItems).ToList();
        }

        public PosTicket AddTicket(IEnumerable<SaleItem> items)
        {
            var ticket = new PosTicket((uint)Tickets.Count + 1);
            ticket.Items.AddRange(items);
            Tickets.Add(ticket);
            return ticket;
        }

        public PosTicket AddTicket(Ticket ticket)
        {
            var posTicket = new PosTicket((uint)ticket.TicketId);
            var saleItems = ticket.Details.Select(d => new SaleItem(d));
            posTicket.Items.AddRange(saleItems);
            Tickets.Add(posTicket);
            return posTicket;
        }

        public decimal TotalFinal => Tickets.Sum(t => t.TotalFinal);

        public IEnumerable<DocumentDetailDto> GetDocumentDetails()
        {
            return SaleItem.GetOrderDetailsFromSaleItems(GetOrderItems());
        }

        public bool ReduceItems(IEnumerable<SaleItem> items)
        {
            return OrdersService.ReduceOrderItems(Id.Value, items);
        }

        public void Close()
        {
            if (Id != null)
            {
                OrdersService.CloseOrder(Id.Value);
            }

            Clear();
        }

        private void Clear()
        {
            Tickets.Clear();
            Id = null;
        }
    }
}
