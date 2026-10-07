using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class ArticleFamilyRepository: Repository.WithDesignation<ArticleFamily>, IArticleFamilyRepository
{
    public ArticleFamilyRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.ArticleSubfamilies!.AnyAsync(x => x.FamilyId == id, ct);
    }
}