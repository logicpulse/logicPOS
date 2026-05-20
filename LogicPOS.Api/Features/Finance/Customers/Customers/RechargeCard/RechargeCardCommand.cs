using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.RechargeCard
{
    public class RechargeCardCommand : IRequest<ErrorOr<Success>>
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }

        public RechargeCardCommand(Guid customerId, decimal amount)
        {
            CustomerId = customerId;
            Amount = amount;
        }
    }
}
