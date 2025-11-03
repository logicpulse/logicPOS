using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.UI.Errors;
using System;
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
                    _paymentMethods = GetAll();
                }
                return _paymentMethods;
            }
        }

        public static List<AutoCompleteLine> AutocompleteLines => PaymentMethods.Select(pm => new AutoCompleteLine
        {
            Id = pm.Id,
            Name = pm.Designation
        }).ToList();

        private static List<PaymentMethod> GetAll()
        {
            var paymentMethods = DependencyInjection.Mediator.Send(new GetAllPaymentMethodsQuery()).Result;

            if (paymentMethods.IsError != false)
            {
                ErrorHandlingService.HandleApiError(paymentMethods);
                return null;
            }

            return paymentMethods.Value.ToList();
        }
        
        public static PaymentMethod GetBydId(Guid id)
        {
            return PaymentMethods.FirstOrDefault(pm => pm.Id == id);
        }
    }
}
