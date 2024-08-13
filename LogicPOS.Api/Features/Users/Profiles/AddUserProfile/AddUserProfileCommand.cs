using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.Profiles.AddUserProfile
{
    public class AddUserProfileCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Notes { get; set;}
    }
}
