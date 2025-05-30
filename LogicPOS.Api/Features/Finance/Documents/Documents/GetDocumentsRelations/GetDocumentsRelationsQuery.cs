using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.GetDocumentsRelations
{
    public class GetDocumentsRelationsQuery : IRequest<ErrorOr<IEnumerable<DocumentRelation>>>
    {
        public IEnumerable<Guid> DocumentIds { get; set; }

        public GetDocumentsRelationsQuery(IEnumerable<Guid> documentIds)
        {
            DocumentIds = documentIds;
        }
    }
}
