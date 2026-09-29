using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.PaymentConditions.DeletePaymentCondition
{
    public class DeletePaymentConditionCommand : DeleteCommand
    {
        public DeletePaymentConditionCommand(Guid id) : base(id)
        {
        }
    }
}
