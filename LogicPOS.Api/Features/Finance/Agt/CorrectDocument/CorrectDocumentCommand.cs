using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.CorrectDocument
{
    public class CorrectDocumentCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid CorrectDocumentId { get; set; }
        public Guid RejectedDocumentId { get; set; }
        public CorrectDocumentCommand(Guid correctDocumentId,Guid rejectedDocumentId)
                                    {
                                        CorrectDocumentId = correctDocumentId;
                                        RejectedDocumentId = rejectedDocumentId;
                                    }
    }
}
