using ErrorOr;
using MediatR;
using System.Collections.Generic;
using System;

namespace LogicPOS.Api.Features.Orders.ReduceItems
{
    public class ReduceOrderItemsCommand : IRequest<ErrorOr<Success>>
    {
        public Guid OrderId { get; set; }
        public List<ReduceOrderItemDto> Items { get; set; }
    }
}
