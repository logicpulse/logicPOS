namespace LogicPOS.Domain.Repositories;

public interface IReceiptRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<string> GetDocumentType(CancellationToken ct = default);
}