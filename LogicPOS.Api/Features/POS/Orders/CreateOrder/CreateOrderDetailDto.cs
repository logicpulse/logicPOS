using System;

namespace LogicPOS.Api.Features.Orders.CreateOrder
{
    public class CreateOrderDetailDto
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
