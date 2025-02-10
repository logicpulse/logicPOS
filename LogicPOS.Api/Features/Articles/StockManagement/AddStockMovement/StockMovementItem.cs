using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement
{
    public class StockMovementItem
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
        public string SerialNumber { get; set; }
        public Guid? WarehouseLocationId { get; set; }
        public decimal Price { get; set; }
    }
}
