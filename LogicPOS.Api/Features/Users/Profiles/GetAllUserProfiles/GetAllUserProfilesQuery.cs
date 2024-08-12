using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles
{
    public class GetAllUserProfilesQuery : IRequest<ErrorOr<IEnumerable<UserProfile>>>
    {
    }
}
