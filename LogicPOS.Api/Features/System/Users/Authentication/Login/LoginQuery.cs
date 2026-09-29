using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Authentication.Login
{
    public class LoginQuery : IRequest<ErrorOr<string>>
    {
        public Guid TerminalId { get; set; }
        public Guid UserId { get; set; }
        public string Pin { get; set; }

        public LoginQuery(Guid terminalId,
                          Guid userId,
                          string pin)
        {
            TerminalId = terminalId;
            UserId = userId;
            Pin = pin;
        }
    }
}
