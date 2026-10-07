using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class PermissionGroupRepository : Repository.WithDesignation<PermissionGroup>, IPermissionGroupRepository
{
    public PermissionGroupRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.PermissionItems!.AnyAsync(x => x.PermissionGroupId == id, ct);
    }
}