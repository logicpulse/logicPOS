using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.GetAllUsers
{
    public class GetAllUsersQueryHandler :
        RequestHandler<GetAllUsersQuery, ErrorOr<IEnumerable<User>>>
    {
        public GetAllUsersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<User>("users",cancellationToken);
        }
    }
}
