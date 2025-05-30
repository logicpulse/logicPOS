using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods
{
    public class GetAllPaymentMethodsQuery : IRequest<ErrorOr<IEnumerable<PaymentMethod>>>
    {
    }
}
