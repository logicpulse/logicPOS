using System;

namespace LogicPOS.Api.Features.Orders.ReduceItems
{
    public class ReduceOrderItemDto
    {
        public Guid ArticleId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
