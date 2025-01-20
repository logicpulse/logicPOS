using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf
{
    public class GetDocumentPdfQuery : IRequest<ErrorOr<string>>
    {
        public GetDocumentPdfQuery(Guid id , uint copyNumber = 1) {
            Id = id;
            CopyNumber = copyNumber;
        }

        public uint CopyNumber { get; set; }
        public Guid Id { get; set; }
    }
}
