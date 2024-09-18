using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes
{
    public class GetAllDocumentTypesQuery : IRequest<ErrorOr<IEnumerable<DocumentType>>>
    {

    }
}
