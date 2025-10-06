using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class Document : ApiEntity
    {
        public Guid CustomerId { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public ShipAddress ShipToAddress { get; set; }
        public ShipAddress ShipFromAddress { get; set; }
        public decimal Discount { get; set; }
        public DocumentCustomer Customer { get; set; }
        public List<DocumentPaymentMethod> PaymentMethods { get; set; }
        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);
    }
}
