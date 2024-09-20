using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries
{
    public class GetAllDocumentSeriesQuery : IRequest<ErrorOr<IEnumerable<DocumentSeries>>>
    {
    }
}
