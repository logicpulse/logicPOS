using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
            try
            {
                var commissionGroups = await _httpClient.GetFromJsonAsync<List<CommissionGroup>>("users/commission-groups", cancellationToken);

                return commissionGroups;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
