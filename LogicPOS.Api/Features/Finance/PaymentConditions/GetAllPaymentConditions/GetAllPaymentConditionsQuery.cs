using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition
{
    public class GetAllPaymentConditionsQuery : IRequest<ErrorOr<IEnumerable<PaymentCondition>>>
    {
    }
}
