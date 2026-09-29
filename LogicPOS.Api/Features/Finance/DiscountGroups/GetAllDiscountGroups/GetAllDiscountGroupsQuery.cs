using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups
{
    public class GetAllDiscountGroupsQuery : IRequest<ErrorOr<IEnumerable<DiscountGroup>>>
    {

    }
}
