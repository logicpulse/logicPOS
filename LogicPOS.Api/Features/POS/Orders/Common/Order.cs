using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.POS.Tables.Common;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.POS.Orders.Orders.Common
{
    public class Order : ApiEntity
    {
        public TableViewModel Table { get; set; }
        public List<Ticket> Tickets { get; set; }
    }

    public class Ticket
    {
        public int TicketId { get; set; }
        public List<OrderDetail> Details { get; set; }
    }

    public class OrderDetail : ApiEntity
    {
        public string Designation { get; set; }
        public ArticleViewModel Article { get;set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalFinal { get; set; }
    }
}
