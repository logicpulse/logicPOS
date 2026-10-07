
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class ArticleCompositionRepository : IArticleCompositionRepository
{
    private readonly LogicPOSDbContext _database;

    public ArticleCompositionRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> CompositionExistsAsync(
        Guid parentId, 
        Guid childId, 
        CancellationToken cancellationToken = default)
    {
        return await _database.ArticleCompositions!
                              .AnyAsync(ac => ac.ParentId == parentId && ac.ChildId == childId,
                                        cancellationToken);
    }

    public Task<bool> IsReferencedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}