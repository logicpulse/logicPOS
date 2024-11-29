using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Users.Profiles.DeleteUserProfile
{
    public class DeleteUserProfileCommand : DeleteCommand
    {
        public DeleteUserProfileCommand(Guid id) : base(id)
        {
        }
    }
}
