using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition;
using LogicPOS.UI.Errors;
using System;
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
                    _paymentConditins = GetAll();
                }

                return _paymentConditins;
            }
        }

        public static List<AutoCompleteLine> AutocompleteLines => PaymentConditions.Select(pc => new AutoCompleteLine
        {
            Id = pc.Id,
            Name = pc.Designation
        }).ToList();

        private static List<PaymentCondition> GetAll()
        {
            var paymentConditions = DependencyInjection.Mediator.Send(new GetAllPaymentConditionsQuery()).Result;

            if (paymentConditions.IsError != false)
            {
                ErrorHandlingService.HandleApiError(paymentConditions);
                return null;
            }

            return paymentConditions.Value.ToList();
        }
        
        public static PaymentCondition GetById(Guid id) => 
            PaymentConditions.FirstOrDefault(pc => pc.Id == id);


    }
}
