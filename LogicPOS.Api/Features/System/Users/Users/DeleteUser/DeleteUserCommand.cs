using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Users.DeleteUser
{
    public class DeleteUserCommand : DeleteCommand
    {
        public DeleteUserCommand(Guid id) : base(id)
        { }
    }
}
