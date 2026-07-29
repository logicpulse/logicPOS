using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.FiscalYears.CloseFiscalYear
{
    public class CloseFiscalYearCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }

        public bool ForceAtCommunication { get; set; } = true;

        public CloseFiscalYearCommand(Guid id, bool forceAtCommunication = true)
        {
            Id = id;
            ForceAtCommunication = forceAtCommunication;
        }
    }
}
