using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.RegisterDocument
{
    public class RegisterDocumentCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid DocumentId { get; set; }
        public RegisterDocumentCommand(Guid documentId) => DocumentId = documentId;
    }
}
