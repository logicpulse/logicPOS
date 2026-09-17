using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ErrorOr<Success>>
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public ResetPasswordCommand(Guid userId,
                                    string oldPassword,
                                    string newPassword)
        {
            UserId = userId;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
