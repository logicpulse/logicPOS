using ErrorOr;
using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Orders.GetAllOrders
{
    public class GetOpenOrdersQuery : IRequest<ErrorOr<IEnumerable<Order>>>
    {
        public Guid? TableId { get; set; }

        public GetOpenOrdersQuery(Guid? tableId = null)
        {
            TableId = tableId;
        }
    }
}
