using ErrorOr;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes
{
    public class GetAllDocumentTypesQuery : IRequest<ErrorOr<IEnumerable<DocumentType>>>
    {

    }
}
