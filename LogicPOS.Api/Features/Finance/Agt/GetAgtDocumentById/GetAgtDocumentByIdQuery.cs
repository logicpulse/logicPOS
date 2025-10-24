using ErrorOr;
using LogicPOS.Api.Features.Finance.Agt.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.GetAgtDocumentById
{
    public class GetAgtDocumentByIdQuery : IRequest<ErrorOr<AgtDocument>>
    {
        public Guid DocumentId { get; set; }

        public GetAgtDocumentByIdQuery(Guid documentId) => DocumentId = documentId;
    }
}
