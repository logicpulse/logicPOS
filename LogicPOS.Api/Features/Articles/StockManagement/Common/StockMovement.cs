using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class StockMovement : ApiEntity
    {
        public Customer Customer { get; set; }
        public Guid? CustomerId { get; set; }

        public Article Article { get; set; }
        public Guid ArticleId { get; set; }

        public ArticleSerialNumber SerialNumber { get; set; }
        public Guid? SerialNumberId { get; set; }

        public string DocumentNumber { get; set; }
        public decimal Quantity { get; set; }
        public DateTime Date { get; set; }
        public DocumentDetail Detail { get; set; }
        public Guid? DetailId { get; set; }
    }
}
