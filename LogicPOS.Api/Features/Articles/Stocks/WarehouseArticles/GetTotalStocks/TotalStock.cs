using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks
{
    public class TotalStock
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
    }
}
