using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateDefaultSeries
{
    public class CreateDefaultSeriesCommand : IRequest<ErrorOr<Success>>
    {
        public Guid FiscalYearId { get; set; }
        public IEnumerable<Guid> Terminals { get; set; }
    }
}
