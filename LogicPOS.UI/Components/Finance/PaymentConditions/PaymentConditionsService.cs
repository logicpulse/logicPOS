using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.PaymentConditions
{
    public class PaymentConditionsService
    {
        private static List<PaymentCondition> _paymentConditins;

        public static List<PaymentCondition> PaymentConditions
        {
            get
            {
                if(_paymentConditins == null)
                {
                    _paymentConditins = GetAllPaymentConditions();
                }

                return _paymentConditins;
            }
        }

        private static List<PaymentCondition> GetAllPaymentConditions()
        {
            var paymentConditions = DependencyInjection.Mediator.Send(new GetAllPaymentConditionsQuery()).Result;

            if (paymentConditions.IsError != false)
            {
                ErrorHandlingService.HandleApiError(paymentConditions);
                return null;
            }

            return paymentConditions.Value.ToList();
        }
    }
}
