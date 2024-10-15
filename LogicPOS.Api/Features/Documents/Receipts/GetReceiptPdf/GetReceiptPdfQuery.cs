using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf
{
    public class GetReceiptPdfQuery : IRequest<ErrorOr<string>>
    {
        public Guid Id { get; set; }
    }
}
