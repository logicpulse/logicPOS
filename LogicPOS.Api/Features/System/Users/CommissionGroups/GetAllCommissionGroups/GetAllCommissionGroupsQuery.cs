using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;


namespace LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups
{
    public class GetAllCommissionGroupsQuery: IRequest<ErrorOr<IEnumerable<CommissionGroup>>>
    {

    }
}
