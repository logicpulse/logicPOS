using System;

namespace LogicPOS.Api.Features.Documents.Documents.AddDocument
{
    public class AddDocumentPaymentMethodDto
    {
        public Guid PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
    }
}
