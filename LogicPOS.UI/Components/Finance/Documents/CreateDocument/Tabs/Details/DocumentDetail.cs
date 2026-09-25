using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Finance;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class DocumentDetail : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public ArticleViewModel Article { get; set; }
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
        public decimal TotalNet => AgtLineRounding.Applies(Article)
            ? AgtLineRounding.Truncate2(Quantity * UnitPrice - DiscountPrice)
            : Quantity * UnitPrice - DiscountPrice;
        public decimal DiscountPrice => Quantity * UnitPrice * Discount / 100M;
        public decimal VatPrice => AgtLineRounding.Applies(Article)
            ? AgtLineRounding.Ceiling2(TotalNet * Vat / 100M)
            : TotalNet * Vat / 100M;
        public string SerialNumber { get; set; }

    }
}
