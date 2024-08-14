using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<UserProfile>>("users/profiles",
                                                                                  cancellationToken);
                return items;
            }
            catch (HttpRequestException)
            {
                return ApiErrors.CommunicationError;
            }
        }
    }
}
