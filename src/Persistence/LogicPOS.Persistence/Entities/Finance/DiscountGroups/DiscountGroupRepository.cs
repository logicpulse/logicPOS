
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class DiscountGroupRepository : Repository.WithDesignation<DiscountGroup>, IDiscountGroupRepository
{
    public DiscountGroupRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        bool result = await Database.Articles!.AnyAsync(x => x.DiscountGroupId == id, ct);
        if(result)
        {
            return true;
        }

        return await Database.ArticleSubfamilies!.AnyAsync(x => x.DiscountGroupId == id, ct);
    }
}