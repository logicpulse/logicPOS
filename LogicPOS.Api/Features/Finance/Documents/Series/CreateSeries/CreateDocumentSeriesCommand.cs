using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateSeries
{
    public class CreateDocumentSeriesCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public int NextNumber { get; set; }
        public int NumberRangeBegin { get; set; }
        public int NumberRangeEnd { get; set; }
        public string Acronym { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid FiscalYearId { get; set; }
        public string AtValidationCode { get; set; }
        public string Notes { get; set; }
        public List<Guid> Terminals { get; set; }
    }
}
