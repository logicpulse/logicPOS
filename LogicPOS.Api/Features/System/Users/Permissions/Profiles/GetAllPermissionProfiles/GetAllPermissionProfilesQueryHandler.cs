using ErrorOr;
using LogicPOS.Api.Errors;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;

namespace LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles
{
    public class GetAllPermissionProfilesQueryHandler :
        RequestHandler<GetAllPermissionProfilesQuery, ErrorOr<IEnumerable<PermissionProfile>>>
    {
        public GetAllPermissionProfilesQueryHandler(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory) { }

        public override async Task<ErrorOr<IEnumerable<PermissionProfile>>> Handle(GetAllPermissionProfilesQuery request,
                                                                       CancellationToken cancellationToken = default)
        {
           return await HandleGetListQueryAsync<PermissionProfile>("users/permission-profiles", cancellationToken);
        }


    }
}
