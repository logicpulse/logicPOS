using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument
{
    public class DocumentDetail
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? VatRateId { get; set; }
        public Guid? VatExemptionId { get; set; }
        public decimal? Discount { get; set; }
        public int? PriceType { get; set; }
        public string Notes { get; set; }
    }
}
