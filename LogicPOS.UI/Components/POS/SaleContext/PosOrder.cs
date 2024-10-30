using LogicPOS.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDetailDto = LogicPOS.Api.Features.Documents.AddDocument.DocumentDetail;


namespace LogicPOS.UI.Components.POS
{
    public class PosOrder
    {
        public Table Table { get; }

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

        public decimal TotalFinal => Tickets.Sum(t => t.TotalFinal);

        public IEnumerable<DocumentDetailDto> GetDocumentDetails()
        {
            var saleItems = GetOrderItems();
            var details = saleItems.Select(item =>
            {
                return new DocumentDetailDto
                {
                    ArticleId = item.Article.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount
                };
            });

            return details;
        }

        public void Clear()
        {
            Tickets.Clear();
        }
    }
}
