using ErrorOr;
using LogicPOS.Api.Features.Orders.CreateOrder;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Orders.AddTicket
{
    public class AddTicketCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid OrderId { get; set; }
        public IEnumerable<CreateOrderDetailDto> Details { get; set; }
    }
}
