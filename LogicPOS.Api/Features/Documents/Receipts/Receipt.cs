using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class Receipt : ApiEntity
    {
        public PaymentMethod PaymentMethod { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string HashCode { get; set; }
        public string Hash4Code { get; set; }
        public string RefNo { get; set; }
        public string TransactionID { get; set; }
        public string TransactionDate { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string StatusReason { get; set; }
        public string StatusSourceID { get; set; }
        public string SourcePayment { get; set; }
        public string Mechanism { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public string SourceID { get; set; }
        public string SystemEntryDate { get; set; }
        public decimal TaxPayable { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal WithholdingTaxAmount { get; set; }
        public string ExtendedValue { get; set; }
    }
}
