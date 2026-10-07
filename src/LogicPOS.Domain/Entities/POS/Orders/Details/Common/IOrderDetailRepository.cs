namespace LogicPOS.Domain.Repositories;

public interface IOrderDetailRepository {
      Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}