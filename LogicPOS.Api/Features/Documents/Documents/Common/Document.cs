using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class Document : ApiEntity
    {
        #region  References
        public PaymentMethod PaymentMethod { get; set; }
        public Guid? PaymentMethodId { get; set; }

        public PaymentCondition PaymentCondition { get; set; }
        public Guid? PaymentConditionId { get; set; }

        public Currency Currency { get; set; }
        public Guid CurrencyId { get; set; }

        public DocumentSeries Series { get; set; }
        public Guid SeriesId { get; set; }

        public Order Order { get; set; }
        public Guid? OrderId { get; set; }

        public Guid? ParentId { get; set; }
        public Guid? ChildId { get; set; }
        #endregion
        public string Type { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string StatusReason { get; set; }
        public string StatusUser { get; set; }
        public string SourceBilling { get; set; }
        public string Hash { get; set; }
        public string HashControl { get; set; }
        public int SelfBillingIndicator { get; set; }
        public int CashVatSchemeIndicator { get; set; }
        public int ThirdPartiesBillingIndicator { get; set; }
        public string EACCode { get; set; }
        public string Date { get; set; }
        public string SystemEntryDate { get; set; }
        public string TransactionID { get; set; }
        public ShipAddress ShipToAdress { get; set; }
        public ShipAddress ShipFromAdress { get; set; }
        public DateTime? MovementStartTime { get; set; }
        public DateTime? MovementEndTime { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalFinalRound { get; set; }
        public decimal TotalDelivery { get; set; }
        public decimal TotalChange { get; set; }
        public string ExternalDocument { get; set; }
        public decimal Discount { get; set; }
        public decimal FinancialDiscount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DocumentCustomer Customer { get; set; }
        public bool Paid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool Printed { get; set; }
        public string ATDocCodeID { get; set; }
        public bool ATResendDocument { get; set; }
        public string ATCUD { get; set; }
        public IList<DocumentDetail> Details { get; set; }
    }
}
