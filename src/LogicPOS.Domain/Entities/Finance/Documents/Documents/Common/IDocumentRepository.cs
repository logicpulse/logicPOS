namespace LogicPOS.Domain.Repositories;

public interface IDocumentRepository
{
    Task<bool> ExistsAsync(Guid documentId, CancellationToken ct = default);
    Task<bool> NumberExistsAsync(string number, CancellationToken ct = default);
    Task<decimal> GetDocumentTotalPaidAsync(Guid documentId, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, decimal>> GetDocumentsTotalPaidAsync(IEnumerable<Guid> documentIds,
        CancellationToken ct = default);
}