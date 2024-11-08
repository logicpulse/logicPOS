using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class DocumentPaymentMethod : ApiEntity
    {
        public Guid PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public decimal Amount { get; set; }
    }
}
