using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateAgtSeries
{
    public class CreateAgtSeriesCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; } 
        public Guid DocumentTypeId { get; set; }
        public Guid FiscalYearId { get; set; }
        public string EstablishmentNumber { get; set; } 
        public string SeriesContingencyIndicator { get; set; }
        public string Notes { get; set; }
    }
}
