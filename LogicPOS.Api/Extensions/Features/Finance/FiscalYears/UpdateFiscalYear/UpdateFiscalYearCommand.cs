using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.FiscalYears.UpdateFiscalYear
{
    public class UpdateFiscalYearCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public int NewYear { get; set; }
        public string NewAcronym { get; set; }
        public bool NewSeriesForEachTerminal { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
