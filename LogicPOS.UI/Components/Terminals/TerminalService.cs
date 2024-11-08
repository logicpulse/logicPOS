using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Terminals.CreateTerminal;
using LogicPOS.Api.Features.Terminals.GetTerminalByHardwareId;
using LogicPOS.Api.Features.Terminals.GetTerminalById;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Terminals
{
    public static class TerminalService
    {
        private const string TERMINAL_HARDWAREID_FILE = "terminal.id";
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public static Terminal Terminal { get; private set; }

        private static async Task<ErrorOr<Guid>> CreateTerminalAsync(CancellationToken ct = default)
        {
            var command = new CreateTerminalCommand();
            return await _mediator.Send(command, ct);
        }

        public static async Task<ErrorOr<Terminal>> InitializeTerminalAsync(CancellationToken ct = default)
        {
            if (HardwareIdFileExists())
            {
                var getTerminal = await _mediator.Send(new GetTerminalByHardwareIdQuery(GetHardwareIdFromFile()), ct);

                if (getTerminal.IsError)
                {
                    return getTerminal.FirstError;
                }

                Terminal = getTerminal.Value;

                if(Terminal == null)
                {
                    var hardwareId = GetHardwareIdFromFile();
                    return Error.NotFound(description:$"Terminal com o HardwareId [{hardwareId}] não econtrado.");
                }

                return Terminal;
            }

            var createTerminalResult = await CreateTerminalAsync(ct);

            if (createTerminalResult.IsError)
            {
                return createTerminalResult.FirstError;
            }

            var getTerminalResult = await _mediator.Send(new GetTerminalByIdQuery(createTerminalResult.Value), ct);

            if (getTerminalResult.IsError)
            {
                return getTerminalResult.FirstError;
            }

            CreateHardwareIdFile(getTerminalResult.Value.HardwareId);

            Terminal = getTerminalResult.Value;

            return Terminal;
        }

        private static bool HardwareIdFileExists()
        {
            return System.IO.File.Exists(TERMINAL_HARDWAREID_FILE);
        }

        private static string GetHardwareIdFromFile()
        {
            return System.IO.File.ReadAllText(TERMINAL_HARDWAREID_FILE);
        }

        private static void CreateHardwareIdFile(string hardwareId)
        {
            System.IO.File.WriteAllText(TERMINAL_HARDWAREID_FILE, hardwareId);
        }

    }
}
