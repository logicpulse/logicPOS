using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using System;

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
        public bool IsCancelled => Status == "A";
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;
        public DateTime? ShipFromAdressDeliveryDate { get; set; }
        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);
        public bool IsDraft { get; set; }
        public decimal TotalFinal { get; set; }
        public bool Paid { get; set; }
    }

    public class Customer : ApiEntity 
    {
        public string FiscalNumber { get; set; }
        public string Name { get; set; }
    }

    public class PaymentCondition : ApiEntity
    {
        public string Designation { get;set; }
    }

    public class Currency : ApiEntity
    {
        public string Designation { get; set; }
    }
}
