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
        RequestHandler<GetAllUsersQuery, ErrorOr<IEnumerable<User>>>
    {
        public GetAllUsersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<User>("users",cancellationToken);
        }
    }
}
