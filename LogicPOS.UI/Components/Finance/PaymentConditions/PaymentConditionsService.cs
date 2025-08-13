using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.PaymentConditions
{
    public class PaymentConditionsService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;
        public static List<PaymentCondition> GetAllPaymentConditions()
        {
            var paymentConditions = _mediator.Send(new GetAllPaymentConditionsQuery()).Result;

            if (paymentConditions.IsError != false)
            {
                ErrorHandlingService.HandleApiError(paymentConditions);
                return null;
            }

            return paymentConditions.Value.ToList();
        }
    }
}
