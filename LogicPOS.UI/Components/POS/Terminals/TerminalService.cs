using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Terminals.CreateTerminal;
using LogicPOS.Api.Features.Terminals.GetAllTerminals;
using LogicPOS.Api.Features.Terminals.GetTerminalByHardwareId;
using LogicPOS.Api.Features.Terminals.GetTerminalById;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Terminals
{
    public static class TerminalService
    {
        private const string TERMINAL_HARDWAREID_FILE = "terminal.id";
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public static Terminal Terminal { get; private set; }
        public static List<Terminal> _terminals;
        public static bool HasThermalPrinter => Terminal != null && Terminal.ThermalPrinter != null;
        public static List<Terminal> Terminals
        {
            get
            {
                if (_terminals == null)
                {
                    _terminals = GetAllTerminals();
                }
                return _terminals;
            }
        }

        private static ErrorOr<Guid> CreateTerminal(string hardwareId)
        {
            var command = new CreateTerminalCommand(hardwareId);
            return _mediator.Send(command).Result;
        }

        public static bool HardwareIdFileExists()
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

        public static ErrorOr<Terminal> InitializeTerminal()
        {
            string hardwareId = GetTerminalHardwareId();

            if (string.IsNullOrWhiteSpace(hardwareId))
            {
                return Error.NotFound(description: "HardwareId não encontrado no serviço de licenciamento.");
            }

            var getTerminalResult = _mediator.Send(new GetTerminalByHardwareIdQuery(hardwareId)).Result;

            if (getTerminalResult.IsError)
            {
                return getTerminalResult.Errors;
            }

            Terminal = getTerminalResult.Value;

            if (Terminal == null && LicensingService.Data.IsValid)
            {
                if (LicensingService.Data.AllNumberOfDevices >= Terminals.Count())
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

                    Terminal = getCreatedTerminal.Value;
                }
                else
                {
                    return Error.Conflict(description: "Limite de Terminais/dispositivos atingido.\n\n" +
                                                       "Entre em contacto com o Suporte Técnico");
                }
            }

            if (Terminal == null)
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

                Terminal = getCreatedTerminal.Value;
            }

            return Terminal;
        }

        public static string GetTerminalHardwareId()
        {
            if (!HardwareIdFileExists())
            {
                CreateHardwareIdFile(Guid.NewGuid().ToString().ToUpper());
            }

            var hardwareId = GetHardwareIdFromFile();
            return hardwareId;
        }

        public static List<Terminal> GetAllTerminals()
        {
            var terminals = _mediator.Send(new GetAllTerminalsQuery()).Result;

            if (terminals.IsError)
            {
                ErrorHandlingService.HandleApiError(terminals);
                return null;
            }

            return terminals.Value.ToList();
        }
    }
}
