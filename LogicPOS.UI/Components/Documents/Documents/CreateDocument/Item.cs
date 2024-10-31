using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class Item : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Article Article { get; set; }
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string VatDesignation { get; set; }
        public decimal Vat { get; set; }
        public Guid VatRateId { get; set; }
        public VatRate VatRate { get; set; }
        public string ExemptionReason { get; set; }
        public VatExemptionReason VatExemptionReason { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalFinal => TotalNet + VatPrice;
        public decimal TotalNet => Quantity * UnitPrice - DiscountPrice;
        public decimal DiscountPrice => Quantity * UnitPrice * Discount / 100;
        public decimal VatPrice => TotalNet * Vat / 100;

    }
}
