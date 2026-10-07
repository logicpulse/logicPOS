using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

namespace LogicPOS.Persistence.Repositories;

public class PermissionProfileRepository : IPermissionProfileRepository
{
      protected readonly LogicPOSDbContext _database;

    public PermissionProfileRepository(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<bool> ProfileExistsAsync(Guid userProfileId,
                                               Guid permissionItemId,
                                               CancellationToken cancellationToken = default)
    {
        var permissionProfile= _database.PermissionProfiles!.FirstOrDefault(x=>x.UserProfileId==userProfileId && x.PermissionItemId==permissionItemId);
        if (permissionProfile != null) 
        { 
           return await Task.FromResult(true);
        }
            
        return false;
    }
}