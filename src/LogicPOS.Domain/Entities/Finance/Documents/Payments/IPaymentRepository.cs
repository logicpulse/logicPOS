namespace LogicPOS.Domain.Repositories;

public interface IPaymentRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaidForDocumentAsync(Guid documentId, CancellationToken ct = default);
}