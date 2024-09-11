using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems
{
    public class GetAllPermissionItemsQueryHandler :
        RequestHandler<GetAllPermissionItemsQuery, ErrorOr<IEnumerable<PermissionItem>>>
    {
        public GetAllPermissionItemsQueryHandler(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory) { }

        public override async Task<ErrorOr<IEnumerable<PermissionItem>>> Handle(GetAllPermissionItemsQuery request,
                                                                       CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<PermissionItem>("users/permission-items", cancellationToken);
        }


    }
}
