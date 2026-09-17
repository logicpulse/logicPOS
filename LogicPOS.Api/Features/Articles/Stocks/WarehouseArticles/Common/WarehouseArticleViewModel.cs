using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common
{
    public class WarehouseArticleViewModel : ApiEntity
    {
        public Guid WarehouseId { get; set; }
        public string Warehouse { get; set; } 
        public Guid LocationId { get; set; }
        public string Location { get; set; } 
        public string Article { get; set; } 
        public string SerialNumber { get; set; }
        public decimal Quantity { get; set; }
        public Guid ArticleId { get; set; }
    }
}
