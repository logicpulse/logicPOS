using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetDetails
{
    public class GetDocumentDetailsQuery : IRequest<ErrorOr<IEnumerable<DocumentDetail>>>
    {
        public Guid DocumentId { get; set; }
        public GetDocumentDetailsQuery(Guid documentId) => DocumentId = documentId;
    }
}
