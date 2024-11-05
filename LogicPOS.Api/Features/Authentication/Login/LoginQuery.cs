using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Authentication.Login
{
    public class LoginQuery : IRequest<ErrorOr<string>>
    {
        public Guid UserId { get; set; }
        public string Pin { get; set; }

        public LoginQuery(Guid userId, string pin)
        {
            UserId = userId;
            Pin = pin;
        }
    }
}
