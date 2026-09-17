using ErrorOr;
using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.POS.Orders.Orders.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<ErrorOr<Order>>
    {
        public Guid Id { get; set; }

        public GetOrderByIdQuery(Guid id) => Id = id;

    }
}
