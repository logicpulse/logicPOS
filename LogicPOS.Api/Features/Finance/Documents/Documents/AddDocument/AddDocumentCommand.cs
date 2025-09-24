using ErrorOr;
using LogicPOS.Api.Features.Documents.Documents.AddDocument;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.AddDocument
{
    public class AddDocumentCommand : IRequest<ErrorOr<Guid>>
    {
        public IEnumerable<DocumentPaymentMethod> PaymentMethods { get; set; }
        public Guid? PaymentConditionId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CurrencyId { get; set; }
        public string Type { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ParentId { get; set; }
        public ShipAddress ShipToAdress { get; set; }
        public ShipAddress ShipFromAdress { get; set; }
        public decimal Discount { get; set; }
        public DocumentCustomer Customer { get; set; } 
        public string Notes { get; set; }
        public List<DocumentDetail> Details { get; set; }
        public decimal? ExchangeRate { get; set; }
        public bool IsDraft { get; set; }
    }
}
