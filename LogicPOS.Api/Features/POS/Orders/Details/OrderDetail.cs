using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class OrderDetail : ApiEntity
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }

        public Article Article { get; set; }
        public Guid ArticleId { get; set; }

        public VatExemptionReason VatExemptionReason { get; set; }
        public Guid? VatExemptionReasonId { get; set; }

        public Ticket Ticket { get; set; }
        public Guid? TicketId { get; set; }

        public decimal Quantity { get; set; }
        public string Unit { get; set; } 
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
    }
}
