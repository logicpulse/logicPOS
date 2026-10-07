namespace LogicPOS.Domain.Repositories;

public interface IWorkSessionMovementRepository 
{
    Task<decimal> GetCashInTerminalAsync(Guid terminalId, CancellationToken ct = default);
}