using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.FreeTable
{
    public class FreeTableCommand : IRequest<ErrorOr<Success>>
    {
        public Guid TableId { get; set; }

        public FreeTableCommand(Guid tableId)
        {
            TableId = tableId;
        }
    }
}
