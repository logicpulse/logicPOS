namespace LogicPOS.Domain.Repositories;

public interface IWorkSessionPeriodRepository 
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> HasAnyOpenDayAsync(CancellationToken cancellationToken = default);

    Task<bool> TerminalHasAnyOpenSessionAsync(Guid terminalId, CancellationToken cancellationToken = default);

    Task<bool> HasAnyOpenTerminalAsync(CancellationToken cancellationToken = default);

    Task<Guid?> GetOpenTerminalSessionIdAsync(Guid terminalId, CancellationToken cancellationToken = default);
    
    Task<Guid?> GetOpenDayIdAsync(CancellationToken cancellationToken = default);
}
