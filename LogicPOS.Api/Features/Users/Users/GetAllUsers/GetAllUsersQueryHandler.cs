using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.GetAllUsers
{
    public class GetAllUsersQueryHandler :
        RequestHandler<GetAllUsersQuery, ErrorOr<IEnumerable<UserDetail>>>
    {
        public GetAllUsersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<UserDetail>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<UserDetail>("users/details",cancellationToken);
        }
    }
}
