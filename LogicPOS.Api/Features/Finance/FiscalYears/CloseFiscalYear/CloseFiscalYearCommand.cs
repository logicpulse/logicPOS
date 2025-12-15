using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.FiscalYears.CloseFiscalYear
{
    public class CloseFiscalYearCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set;  }

        public CloseFiscalYearCommand(Guid id) => Id = id;
    }
}
