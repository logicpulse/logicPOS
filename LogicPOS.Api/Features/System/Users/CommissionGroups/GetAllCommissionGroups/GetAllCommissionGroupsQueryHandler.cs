using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups
{
    public class GetAllCommissionGroupsQueryHandler : RequestHandler<GetAllCommissionGroupsQuery, ErrorOr<IEnumerable<CommissionGroup>>>
    {
        public GetAllCommissionGroupsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<CommissionGroup>>> Handle(
            GetAllCommissionGroupsQuery request,
            CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<CommissionGroup>("users/commission-groups", cancellationToken);
        }
    }
}
