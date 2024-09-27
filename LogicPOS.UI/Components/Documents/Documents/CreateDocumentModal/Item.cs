using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
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
        public VatRate VatRate { get; set; }
        public VatExemptionReason VatExemptionReason { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalNet { get;  set; }
    }
}
