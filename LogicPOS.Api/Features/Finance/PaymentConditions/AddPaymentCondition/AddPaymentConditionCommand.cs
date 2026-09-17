using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.PaymentConditions.AddPaymentCondition
{
    public class AddPaymentConditionCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation {  get; set; }
        public string Acronym { get; set; }
        public string Notes { get; set; }
    }
}
