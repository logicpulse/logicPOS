using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Receipts.CancelReceipt
{
    public class CancelReceiptCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
