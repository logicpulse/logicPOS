namespace LogicPOS.Domain.Repositories;

public interface IDocumentDetailRepository {
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}