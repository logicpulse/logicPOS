using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.CancelDocument
{
    public class CancelDocumentCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
