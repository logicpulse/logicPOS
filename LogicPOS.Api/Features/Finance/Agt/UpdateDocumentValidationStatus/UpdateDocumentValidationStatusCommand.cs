using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.UpdateDocumentValidationStatus
{
    public class UpdateDocumentValidationStatusCommand : IRequest<ErrorOr<Success>>
    {
        public Guid DocumentId { get; set; }
        public UpdateDocumentValidationStatusCommand(Guid documentId) => DocumentId = documentId;
    }
}
