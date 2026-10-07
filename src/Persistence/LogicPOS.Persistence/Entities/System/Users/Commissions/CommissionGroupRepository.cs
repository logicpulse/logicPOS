using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Entities.System.Users.Commissions;

public class CommissionGroupRepository : Repository.WithDesignation<CommissionGroup>, ICommissionGroupRepository
{
    public CommissionGroupRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Database.Users!.AnyAsync(x => x.CommissionGroupId == id, ct);
        if (result)
        {
            return true;
        }
    
        result = await Database.ArticleSubfamilies!.AnyAsync(x => x.CommissionGroupId == id, ct);
        if (result)
        {
            return true;
        }

        result = await Database.ArticleFamilies!.AnyAsync(x => x.CommissionGroupId == id, ct);
        if (result)
        {
            return true;
        }

        return await Database.Articles!.AnyAsync(x => x.CommissionGroupId == id, ct);
    }
}