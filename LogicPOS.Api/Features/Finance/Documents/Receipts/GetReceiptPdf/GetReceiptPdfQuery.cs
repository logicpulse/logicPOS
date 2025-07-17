using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf
{
    public class GetReceiptPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public Guid Id { get; set; }
        public uint CopyNumber { get; set; }

        public GetReceiptPdfQuery(Guid id, uint copyNumber)
        {
            Id = id;
            CopyNumber = copyNumber;
        }
    }
}
