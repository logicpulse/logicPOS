using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.Permissions.Profiles.DeletePermissionProfile
{
    public class DeletePermissionProfileCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
    }
}
