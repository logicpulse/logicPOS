using ErrorOr;
using MediatR;
using System;
using LogicPOS.Api.Features.Orders.ReduceItems;

namespace LogicPOS.Api.Features.Orders.RemoveTicketItem
{
    public class MoveTicketItemCommand : IRequest<ErrorOr<Success>>
    {

        public Guid OrderId { get; set; }
        public Guid TableId { get; set; }
        public ReduceOrderItemDto Item { get; set; }
    }
}
