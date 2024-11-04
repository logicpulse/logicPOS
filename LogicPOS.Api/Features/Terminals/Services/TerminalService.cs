using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Terminals.CreateTerminal;
using LogicPOS.Api.Features.Terminals.GetTerminalByHardwareId;
using LogicPOS.Api.Features.Terminals.GetTerminalById;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.Services
{
    public class TerminalService : ITerminalService
    {
        private const string TERMINAL_HARDWAREID_FILE = "terminal.id";
        private readonly IMediator _mediator;

        public TerminalService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ErrorOr<Guid>> CreateTerminalAsync(CancellationToken ct = default)
        {
            var command = new CreateTerminalCommand();
            return await _mediator.Send(command, ct);
        }

        public async Task<ErrorOr<Terminal>> GetCurrentTerminalAsync(CancellationToken ct = default)
        {
            if(HardwareIdFileExists())
            {
                return await _mediator.Send(new GetTerminalByHardwareIdQuery(GetHardwareIdFromFile()), ct);
            }

            var createTerminalResult = await CreateTerminalAsync(ct);

            if (createTerminalResult.IsError)
            {
                return createTerminalResult.FirstError;
            }

            var terminal = await _mediator.Send(new GetTerminalByIdQuery(createTerminalResult.Value), ct);

            if (terminal.IsError)
            {
                return terminal.FirstError;
            }

            CreateHardwareIdFile(terminal.Value.HardwareId);

            return terminal;
        }

        private bool HardwareIdFileExists()
        {
            return System.IO.File.Exists(TERMINAL_HARDWAREID_FILE);
        }

        private string GetHardwareIdFromFile()
        {
            return System.IO.File.ReadAllText(TERMINAL_HARDWAREID_FILE);
        }

        private void CreateHardwareIdFile(string hardwareId)
        {
            System.IO.File.WriteAllText(TERMINAL_HARDWAREID_FILE, hardwareId);
        }

    }
}
