using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.PaymentMethods.DeletePaymentMethod
{
    public class DeletePaymentMethodCommand : DeleteCommand
    {
        public DeletePaymentMethodCommand(Guid id) : base(id)
        {
        }
    }
}
