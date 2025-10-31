using ErrorOr;
using MediatR;
using System.Collections.Generic;
using System;

namespace LogicPOS.Api.Features.Orders.ChangeOrderTable
{
    public class ChangeOrderTableCommand : IRequest<ErrorOr<Success>>
    {
        public Guid OrderId { get; set; }
        public Guid NewTableId { get; set; }
    }
}
