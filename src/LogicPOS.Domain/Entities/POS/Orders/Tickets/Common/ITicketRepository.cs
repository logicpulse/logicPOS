namespace LogicPOS.Domain.Repositories;

public interface ITicketRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextTicketIdForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}