using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories
{
    public class ArticleHistory : ApiEntity
    {
        public string Article { get; set; }
        public decimal ArticlePrice { get; set; }
        public Guid ArticleId { get; set; }
        public string SerialNumber { get; set; }
        public string Warehouse { get; set; }
        public string WarehouseLocation { get; set; }
        public Guid WarehouseLocationId { get; set; }
        public ArticleSerialNumberStatus Status { get; set; }
        public bool IsComposed { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? SaleDate { get; set; }
        public string OriginDocument { get; set; }
        public string SaleDocument { get; set; }
        public Guid InMovementId { get; set; }
        public Guid? OutMovementId { get; set; }
        public string Supplier { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool HasExternalDocument { get; set; }
    }
}
