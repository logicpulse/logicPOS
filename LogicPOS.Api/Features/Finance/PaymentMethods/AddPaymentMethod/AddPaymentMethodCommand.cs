using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.PaymentMethods.AddPaymentMethod
{
    public class AddPaymentMethodCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation {  get; set; }
        public string Token { get; set; }
        public string Acronym { get; set; }
        public string Notes { get; set; }
    }
}
