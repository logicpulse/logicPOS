using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.FiscalYears.AddFiscalYear
{
    public class AddFiscalYearCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation {  get; set; }
        public int Year { get; set; }
        public string Acronym { get; set; }
        public bool SeriesForEachTerminal { get; set; }
        public string Notes { get; set; }
    }
}
