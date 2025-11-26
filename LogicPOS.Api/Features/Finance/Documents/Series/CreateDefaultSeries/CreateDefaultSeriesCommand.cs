using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateDefaultSeries
{
    public class CreateDefaultSeriesCommand : IRequest<ErrorOr<Success>>
    {
        public Guid FiscalYearId { get; set; }

        public CreateDefaultSeriesCommand(Guid fiscalYearId) => FiscalYearId = fiscalYearId;

    }
}
