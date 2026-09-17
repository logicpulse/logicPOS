using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Terminals.CreateTerminal
{
    public class CreateTerminalCommand : IRequest<ErrorOr<Guid>>
    {
        public string HardwareId { get; set; }

        public CreateTerminalCommand(string hardwareId)
        {
            HardwareId = hardwareId;
        }
    }
}
