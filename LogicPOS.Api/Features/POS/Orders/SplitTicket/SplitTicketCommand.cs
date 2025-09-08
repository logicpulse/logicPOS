using ErrorOr;
using MediatR;
using System.Collections.Generic;
using System;

namespace LogicPOS.Api.Features.Orders.SplitTicket
{
    public class SplitTicketCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid OrderId { get; set; }
        public int SplittersNumber { get; set; }
        public List<SplitTicketDto> Items { get; set; }
    }
}
