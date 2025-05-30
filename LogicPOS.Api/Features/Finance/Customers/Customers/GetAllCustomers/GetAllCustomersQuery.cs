using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Customers.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<ErrorOr<IEnumerable<Customer>>>
    {
    }
}
