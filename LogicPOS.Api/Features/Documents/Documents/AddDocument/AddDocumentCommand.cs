using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.AddDocument
{
    public class AddDocumentCommand : IRequest<ErrorOr<Guid>>
    {

    }
}
