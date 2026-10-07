using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

    public class PermissionItemRepository: Repository<PermissionItem>, IPermissionItemRepository
    {
    public PermissionItemRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.PermissionProfiles!.AnyAsync(x => x.PermissionItemId == id, ct);
    }
}

