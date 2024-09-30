using System;

namespace LogicPOS.Api.Features.Documents.AddDocument
{
    public class DocumentDetail
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string VatDesignation { get; set; } 
        public decimal? VatRate { get; set; }
        public string VatExemptionReason { get; set; }
        public decimal? Discount { get; set; }
        public int? PriceType { get; set; }
    }
}
