using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.At.RegisterDocument
{
    public class RegisterDocumentCommand : IRequest<ErrorOr<RegisterDocumentResponse>>
    {
        public Guid DocumentId { get; set; }

        public RegisterDocumentCommand(Guid documentId) => DocumentId = documentId;
    }
}
