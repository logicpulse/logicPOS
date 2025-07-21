using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.GetUserPermissions
{
    public class GetUserPermissionQueryHandler :
        RequestHandler<GetUserPermissionsQuery, ErrorOr<IEnumerable<string>>>
    {
        public GetUserPermissionQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<string>>> Handle(GetUserPermissionsQuery query,
                                                                 CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<string>($"users/{query.Id}/permissions", cancellationToken);
        }
    }
}
