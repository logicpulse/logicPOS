using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.GetDocumentsTotals
{
    public class GetDocumentsTotalsQuery : IRequest<ErrorOr<IEnumerable<DocumentTotals>>>
    {
    }
}
