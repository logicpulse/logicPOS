namespace LogicPOS.Domain.Repositories;

public interface IOrderRepository {
      Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}