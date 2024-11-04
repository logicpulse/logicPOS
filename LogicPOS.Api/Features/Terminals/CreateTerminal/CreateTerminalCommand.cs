using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Terminals.CreateTerminal
{
    public class CreateTerminalCommand : IRequest<ErrorOr<Guid>>
    {
    }
}
