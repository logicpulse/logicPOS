using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.Permissions.Profiles.AddPermissionProfile
{
    public class AddPermissionProfileCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid PermissionItemId { get; set; } 
        public Guid UserProfileId { get; set; } 
    }
}
