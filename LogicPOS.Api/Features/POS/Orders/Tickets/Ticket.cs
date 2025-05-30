using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class Ticket : ApiEntity
    {
        public Order Order { get; set; }
        public Guid OrderId { get; set; }

        public int TicketId { get; set; }
        public Api.Enums.PriceType PriceType { get; set; }
        public decimal Discount { get; set; }

        public IList<OrderDetail> Details { get; set; }
    }
}
