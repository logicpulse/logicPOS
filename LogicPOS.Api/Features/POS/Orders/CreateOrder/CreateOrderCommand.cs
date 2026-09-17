using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid TableId { get; set; }
        public IEnumerable<CreateTicketDto> Tickets { get; set; } 
    }
}
