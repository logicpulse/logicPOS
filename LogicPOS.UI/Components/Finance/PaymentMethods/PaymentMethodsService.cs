using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.PaymentMethods
{
    public class PaymentMethodsService
    {
        private static List<PaymentMethod> _paymentMethods;

        public static List<PaymentMethod> PaymentMethods
        {
            get
            {
                if (_paymentMethods == null)
                {
                    _paymentMethods = GetAllPaymentMethods();
                }
                return _paymentMethods;
            }
        }

        private static List<PaymentMethod> GetAllPaymentMethods()
        {
            var paymentMethods = DependencyInjection.Mediator.Send(new GetAllPaymentMethodsQuery()).Result;

            if (paymentMethods.IsError != false)
            {
                ErrorHandlingService.HandleApiError(paymentMethods);
                return null;
            }

            return paymentMethods.Value.ToList();
        }
    }
}
