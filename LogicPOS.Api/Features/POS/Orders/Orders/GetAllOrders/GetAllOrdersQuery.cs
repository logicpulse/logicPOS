using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Orders.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<ErrorOr<IEnumerable<Order>>>
    {
        public Guid? TableId { get; set; }

        public GetAllOrdersQuery(Guid? tableId = null)
        {
            TableId = tableId;
        }
    }
}
