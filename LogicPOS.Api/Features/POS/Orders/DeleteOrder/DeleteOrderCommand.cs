using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.POS.Orders.Orders.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<ErrorOr<bool>>
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }

        public DeleteOrderCommand(Guid orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }

        public string GetUrlQuery()
        {
            return $"?Delete=true&&Reason={Reason}";
        }
    }
}
