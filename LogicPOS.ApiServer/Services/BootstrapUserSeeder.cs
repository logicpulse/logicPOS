using LogicPOS.ApiServer.Authentication;
using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LogicPOS.ApiServer.Services;

public sealed class BootstrapUserSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BootstrapUserSettings _bootstrapUserSettings;
    private readonly PinHasher _pinHasher;

    public BootstrapUserSeeder(
        ApplicationDbContext dbContext,
        IOptions<BootstrapUserSettings> bootstrapUserOptions,
        PinHasher pinHasher)
    {
        _dbContext = dbContext;
        _bootstrapUserSettings = bootstrapUserOptions.Value;
        _pinHasher = pinHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_bootstrapUserSettings.IsConfigured)
        {
            return;
        }

        var existingUser = await _dbContext.ApiUsers
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == _bootstrapUserSettings.UserId, cancellationToken);

        if (existingUser is not null)
        {
            return;
        }

        var (hash, salt) = _pinHasher.Hash(_bootstrapUserSettings.Pin);

        _dbContext.ApiUsers.Add(new ApiUser
        {
            Id = _bootstrapUserSettings.UserId,
            TerminalId = _bootstrapUserSettings.TerminalId,
            Username = _bootstrapUserSettings.Username,
            PinHash = hash,
            PinSalt = salt,
            CreatedUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
