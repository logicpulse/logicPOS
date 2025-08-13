using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.PaymentMethods
{
    public class PaymentMethodsService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;

        public static List<PaymentMethod> GetAllPaymentMethods()
        {
            var paymentMethods = _mediator.Send(new GetAllPaymentMethodsQuery()).Result;

            if (paymentMethods.IsError != false)
            {
                ErrorHandlingService.HandleApiError(paymentMethods);
                return null;
            }

            return paymentMethods.Value.ToList();
        }
    }
}
