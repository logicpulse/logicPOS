using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class ArticleSubfamilyRepository : Repository.WithDesignation<ArticleSubfamily>, IArticleSubfamilyRepository
{
    public ArticleSubfamilyRepository(LogicPOSDbContext database) : base(database)
    {
    }
    
    public async  Task<bool> DesignationExistsAsync(Guid familyId, string designation, CancellationToken ct)
    {
        return await Database.ArticleSubfamilies!.AnyAsync(x => x.Designation.ToLower() == designation.ToLower() && x.FamilyId == familyId, ct);
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Articles!.AnyAsync(x => x.SubfamilyId == id, ct);
    }
}