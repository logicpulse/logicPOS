using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Customers.Types.GetAllCustomerTypes
{
    public class GetAllCustomerTypesQuery : IRequest<ErrorOr<IEnumerable<CustomerType>>>
    {

    }
}
