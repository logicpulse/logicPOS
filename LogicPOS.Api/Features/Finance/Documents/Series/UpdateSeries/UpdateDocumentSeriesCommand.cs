using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Series.UpdateDocumentSerie
{
    public class UpdateDocumentSeriesCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public string Designation { get; set; }
        public string AtValidationCode { get; set; }
    }
}
