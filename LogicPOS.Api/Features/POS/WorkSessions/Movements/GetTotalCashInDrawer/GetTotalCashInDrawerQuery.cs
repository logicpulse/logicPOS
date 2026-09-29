using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.Movements.GetTotalCashInDrawer
{
    public class GetTotalCashInDrawerQuery : IRequest<ErrorOr<decimal?>>
    {
        public Guid TerminalId { get; set; }

        public GetTotalCashInDrawerQuery(Guid terminalId)
        {
            TerminalId = terminalId;
        }
    }
}
