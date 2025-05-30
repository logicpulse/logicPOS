using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.PayDocuments
{
    public class PayDocumentsCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid PaymentMethodId { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public IEnumerable<Guid> Documents { get; set; } 

    }
}
