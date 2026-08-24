using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentFromOrder
{
    public class GetDocumentFromOrderQuery : IRequest<ErrorOr<bool>>
    {
        public GetDocumentFromOrderQuery(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; }
    }
}
