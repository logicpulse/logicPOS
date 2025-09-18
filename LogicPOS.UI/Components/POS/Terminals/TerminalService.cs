using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Terminals.CreateTerminal;
using LogicPOS.Api.Features.Terminals.GetTerminalByHardwareId;
using LogicPOS.Api.Features.Terminals.GetTerminalById;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using static LogicPOS.UI.Application.Licensing.LicenseRouter;

namespace LogicPOS.UI.Components.Terminals
{
    public static class TerminalService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public static Terminal Terminal { get; private set; }

        public static bool HasThermalPrinter => Terminal != null && Terminal.ThermalPrinter != null;

        private static ErrorOr<Guid> CreateTerminal(string hardwareId)
        {
            var command = new CreateTerminalCommand(hardwareId);
            return _mediator.Send(command).Result;
        }

        public static ErrorOr<Terminal> InitializeTerminal()
        {
            var hardwareId = GlobalFramework.LicenceHardwareId;

            if (string.IsNullOrWhiteSpace(hardwareId))
            {
                return Error.NotFound(description: "HardwareId não encontrado no GlobalFramework.");
            }

            var getTerminalResult = _mediator.Send(new GetTerminalByHardwareIdQuery(hardwareId)).Result;

            if (getTerminalResult.IsError)
            {
                return getTerminalResult.Errors;
            }

            var terminal = getTerminalResult.Value;

            if (terminal == null)
            {
                var createTerminalResult = CreateTerminal(hardwareId);

                if (createTerminalResult.IsError)
                {
                    return createTerminalResult.Errors;
                }

                var getCreatedTerminal = _mediator.Send(new GetTerminalByIdQuery(createTerminalResult.Value)).Result;

                if (getCreatedTerminal.IsError)
                {
                    return getCreatedTerminal.FirstError;
                }

                terminal = getCreatedTerminal.Value;
            }

            Terminal = terminal;
            return Terminal;
        }
    }
}
