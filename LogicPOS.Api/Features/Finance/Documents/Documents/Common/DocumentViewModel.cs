using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Finance.Documents.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Common
{
    public class DocumentViewModel : ApiEntity
    {
        public PaymentCondition PaymentCondition { get; set; }
        public Currency Currency { get; set; }
        public Customer Customer { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public AgtInfo AgtInfo { get; set; }
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;
        public DateTime? ShipFromAddressDeliveryDate { get; set; }
        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);
        public bool IsDraft { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalToPay { get; set; }
        public string AtDocCodeId { get; set; }
        public bool AtResendDocument { get; set; }
        public List<string> RelatedDocuments { get; set; }
        public bool Paid { get; set; }
        public bool IsActive => Status == "N" && IsDraft == false;
        public bool IsCancellable => IsActive && HasPassed48Hours == false;
        public bool IsPayable => IsActive && Paid == false && (TypeAnalyzer.IsInvoice() || TypeAnalyzer.IsDebitNote());
        public bool IsAgtDocument => (TypeAnalyzer.IsInvoice() || TypeAnalyzer.IsInvoiceReceipt() || TypeAnalyzer.IsCreditNote() || TypeAnalyzer.IsDebitNote()) && IsDraft == false;
        public bool IsAtDocument => TypeAnalyzer.IsWayBill() && IsDraft == false;
        public string GetAgtStatus()
        {
            if (IsAgtDocument == false)
            {
                return "N/A";
            }

            if (AgtInfo == null || string.IsNullOrWhiteSpace(AgtInfo.RequestId))
            {
                return "Não submetido";
            }

            if (string.IsNullOrWhiteSpace(AgtInfo.ValidationStatus))
            {
                return "Submetido (Não validado)";
            }

            return $"Submetido ({AgtInfo.ValidationStatus})";
        }

        public string GetAtStatus()
        {
            if (TypeAnalyzer.IsWayBill() == false)
            {
                return "N/A";

            }

            return (AtResendDocument == false || string.IsNullOrWhiteSpace(AtDocCodeId) == false) ? $"Comunicado ({AtDocCodeId})" : "Não comunicado";
        }
    }

    public class Customer : ApiEntity
    {
        public string FiscalNumber { get; set; }
        public string Name { get; set; }
    }

    public class PaymentCondition : ApiEntity
    {
        public string Designation { get; set; }
    }

    public class Currency : ApiEntity
    {
        public string Designation { get; set; }
    }
}
