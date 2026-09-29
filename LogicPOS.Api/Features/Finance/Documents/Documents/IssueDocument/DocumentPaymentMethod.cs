using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument
{
    public class DocumentPaymentMethod
    {
        public Guid PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
    }
}
