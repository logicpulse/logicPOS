using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles
{
    public class GetAllUserProfilesQueryHandler :
        RequestHandler<GetAllUserProfilesQuery, ErrorOr<IEnumerable<UserProfile>>>
    {
        public GetAllUserProfilesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<IEnumerable<UserProfile>>> Handle(GetAllUserProfilesQuery request,
                                                                             CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQuery<UserProfile>("users/profiles", cancellationToken);
        }
    }
}
