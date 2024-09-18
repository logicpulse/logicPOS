using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Series.AddDocumentSerie
{
    public class AddDocumentSerieCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public int NextNumber { get; set; }
        public int NumberRangeBegin { get; set; }
        public int NumberRangeEnd { get; set; }
        public string Acronym { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid FiscalYearId { get; set; }
        public string ATDocCodeValidationSerie { get; set; }
        public string Notes { get; set; }
    }
}
