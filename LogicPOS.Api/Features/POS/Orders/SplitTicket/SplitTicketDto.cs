using System;

namespace LogicPOS.Api.Features.Orders.SplitTicket
{
    public class SplitTicketDto
    {
        public Guid ArticleId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
