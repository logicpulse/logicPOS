
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class UserProfileRepository : Repository.WithDesignation<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Users!.AnyAsync(x => x.ProfileId == id, ct);
    }
}