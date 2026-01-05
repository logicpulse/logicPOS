using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPrint
{
    public class GetDocumentPrintQuery : IRequest<ErrorOr<DocumentPrint>>
    {
        public Guid DocumentId { get; set; }

        public GetDocumentPrintQuery(Guid id)
        {
            DocumentId = id;
        }
    }
}
