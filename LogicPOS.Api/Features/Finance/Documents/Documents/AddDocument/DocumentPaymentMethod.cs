using System;

namespace LogicPOS.Api.Features.Documents.Documents.AddDocument
{
    public class DocumentPaymentMethod
    {
        public Guid PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
    }
}
