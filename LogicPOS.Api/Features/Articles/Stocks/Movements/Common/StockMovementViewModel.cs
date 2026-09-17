using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Stocks.Common
{
    public sealed class StockMovementViewModel : ApiEntity
    {
        public string Customer { get; set; }
        public Guid? CustomerId { get; set; }
        public string Article { get; set; }
        public Guid ArticleId { get; set; }
        public string SerialNumber { get; set; }
        public string DocumentNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public bool HasExternalDocument { get; set; }
    }
}
