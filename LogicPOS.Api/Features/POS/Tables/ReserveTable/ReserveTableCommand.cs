using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.ReserveTable
{
    public class ReserveTableCommand : IRequest<ErrorOr<Success>>
    {
        public Guid TableId { get; set; }

        public ReserveTableCommand(Guid tableId)
        {
            TableId = tableId;
        }
    }
}
