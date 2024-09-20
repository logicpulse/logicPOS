using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Orders;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class Order : ApiEntity
    {
        public Table Table { get; set; }
        public Guid TableId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public IList<Ticket> Tickets { get; set; }
    }
}
