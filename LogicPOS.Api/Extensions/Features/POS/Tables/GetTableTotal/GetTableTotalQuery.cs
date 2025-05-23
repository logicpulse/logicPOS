using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.GetTableTotal
{
    public class GetTableTotalQuery : IRequest<ErrorOr<decimal>>
    {
        public Guid TableId { get; set; }

        public GetTableTotalQuery(Guid tableId)
        {
            TableId = tableId;
        }
    }
}
