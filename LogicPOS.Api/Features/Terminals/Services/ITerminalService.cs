using ErrorOr;
using LogicPOS.Api.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Terminals.Services
{
    public interface ITerminalService
    {
        Task<ErrorOr<Guid>> CreateTerminalAsync(CancellationToken ct = default);

        Task<ErrorOr<Terminal>> GetCurrentTerminalAsync(CancellationToken ct = default);
    }
}
