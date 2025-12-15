using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries
{
    public class GetActiveDocumentSeriesQuery : IRequest<ErrorOr<IEnumerable<DocumentSeries>>>
    {
    }
}
