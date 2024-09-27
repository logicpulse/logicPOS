using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.GetAllDocuments
{
    public class GetAllDocumentsQuery : IRequest<ErrorOr<IEnumerable<Document>>>
    {

    }
}
