using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf
{
    public class GetDocumentPdfQuery : IRequest<ErrorOr<string>>
    {
        public Guid Id { get; set; }
    }
}
