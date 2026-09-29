using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Orders.CloseOrder
{
    public class CloseOrderCommand : IRequest<ErrorOr<bool>>
    {
        public Guid OrderId { get; set; }
    }
}
