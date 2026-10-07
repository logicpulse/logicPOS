namespace LogicPOS.Domain.Repositories;

public interface IArticleCompositionRepository
{
    public Task<bool> CompositionExistsAsync(Guid parentId,
                                             Guid childId,
                                             CancellationToken cancellationToken = default);

    Task<bool> IsReferencedAsync(Guid id, CancellationToken cancellationToken = default);

}