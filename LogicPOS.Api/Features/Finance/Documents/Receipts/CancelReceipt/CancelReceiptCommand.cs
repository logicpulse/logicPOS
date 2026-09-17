using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Receipts.CancelReceipt
{
    public class CancelReceiptCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
