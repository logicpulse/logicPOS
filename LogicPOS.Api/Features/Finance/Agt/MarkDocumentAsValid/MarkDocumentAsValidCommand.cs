using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.MarkDocumentAsValid
{
    public class MarkDocumentAsValidCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }

        public MarkDocumentAsValidCommand(Guid id) => Id = id;

    }
}
