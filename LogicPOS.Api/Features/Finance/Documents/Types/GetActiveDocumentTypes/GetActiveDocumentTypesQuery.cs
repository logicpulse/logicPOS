using ErrorOr;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Types.GetActiveDocumentTypes
{
    public class GetActiveDocumentTypesQuery : IRequest<ErrorOr<IEnumerable<DocumentType>>>
    {

    }
}
