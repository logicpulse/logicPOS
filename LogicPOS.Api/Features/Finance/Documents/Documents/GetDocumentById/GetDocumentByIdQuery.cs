using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.GetDocumentById
{
    public class GetDocumentByIdQuery : IRequest<ErrorOr<Document>>
    {
        public Guid Id { get; }
        public GetDocumentByIdQuery(Guid id) => Id = id;
    }
}
