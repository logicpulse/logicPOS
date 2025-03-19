using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class ArticleHistory : ApiEntity
    {
        public WarehouseArticle WarehouseArticle { get; set; } 
        public StockMovement InStockMovement { get; set; } 
        public StockMovement OutStockMovement { get; set; }

        public new Guid Id => WarehouseArticle.Id;
        public new DateTime CreatedAt => WarehouseArticle.CreatedAt;
        public new DateTime UpdatedAt => WarehouseArticle.UpdatedAt;
        public new Guid UpdatedBy => WarehouseArticle.UpdatedBy;
    }
}
