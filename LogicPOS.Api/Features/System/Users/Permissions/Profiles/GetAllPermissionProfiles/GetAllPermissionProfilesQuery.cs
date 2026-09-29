using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles
{
    public class GetAllPermissionProfilesQuery : IRequest<ErrorOr<IEnumerable<PermissionProfile>>>
    {

    }
}
