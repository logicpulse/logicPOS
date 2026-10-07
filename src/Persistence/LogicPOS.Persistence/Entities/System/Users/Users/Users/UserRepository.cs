using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public override Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Database.Users!.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await Database.Users.AnyAsync(x => x.Email != null && x.Email.ToLower() == email.ToLower(), ct);
    }

    public async Task<bool> LoginExistsAsync(string login, CancellationToken ct = default)
    {
        return await Database.Users.AnyAsync(x => x.Login != null && x.Login.ToLower() == login.ToLower(), ct);
    }
}
