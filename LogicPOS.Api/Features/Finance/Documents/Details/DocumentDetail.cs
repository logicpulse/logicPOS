using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.Details;
using System;

namespace LogicPOS.Api.Entities
{
    public class DocumentDetail : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public Article Article { get; set; }
        public Guid ArticleId { get; set; }

        public Document Document { get; set; }
        public Guid DocumentId { get; set; }

        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public DocumentDetailTax Tax { get; set; }
        public string VatExemptionReason { get; set; }
        public string VatExemptionCode { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        public Api.Enums.PriceType? PriceType { get; set; }
        public string Token1 { get; set; }
        public string Token2 { get; set; }
        public string Warehouse { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }

    }
}
