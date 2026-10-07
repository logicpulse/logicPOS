namespace LogicPOS.Domain.Repositories;

public interface IPrinterAssociationRepository
{
    public Task<bool> EntityExistsAsync(Guid entityId, CancellationToken cancellationToken = default);
    public Task<bool> AssociationExistsAsync(Guid printerId, Guid entityId, CancellationToken cancellationToken = default);
    public Task RemoveEntityAssociationAsync(Guid entityId, CancellationToken cancellationToken = default);
    public Task<Guid> AssociatePrinterAsync(Guid printerId, Guid entityId, CancellationToken cancellationToken = default);
}
