using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class UsersService
{
    private readonly ApplicationDbContext _dbContext;

    public UsersService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.ApiUsers
            .AsNoTracking()
            .OrderBy(user => user.Username)
            .ToListAsync(cancellationToken);

        return users.Select((user, index) => Map(user, (uint)(index + 1))).ToList();
    }

    public async Task<string?> GetNameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiUsers
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => user.Username)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<IReadOnlyList<string>> GetPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }

    private static UserResponse Map(Data.Entities.ApiUser user, uint order)
    {
        return new UserResponse
        {
            Id = user.Id,
            Notes = string.Empty,
            CreatedAt = user.CreatedUtc,
            UpdatedAt = user.CreatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = false,
            ProfileId = Guid.Empty,
            CommissionGroupId = null,
            Name = string.IsNullOrWhiteSpace(user.Username) ? user.Id.ToString() : user.Username,
            Login = user.Username,
            Code = $"U{order}",
            Order = order,
            PasswordReset = false
        };
    }
}
