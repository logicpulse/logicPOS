using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentWasPrinted
{
    public class GetDocumentWasPrintedQuery : IRequest<ErrorOr<bool>>
    {
        public GetDocumentWasPrintedQuery(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; }
    }
}
