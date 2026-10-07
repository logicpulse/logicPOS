using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Permissions.Groups;

public static class PermissionGroupsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding permission groups");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "permissiongroups.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Permission groups seed file not found.");
            return false;
        }

        try
        {
            List<PermissionGroup>? permissionGroups = JsonSerializer.Deserialize<List<PermissionGroup>>(File.ReadAllText(seedFilePath));
            if (permissionGroups is null)
            {
                logger.LogError("Permission groups seed file is empty.");
                return false;
            }
            database.PermissionGroups.AddRange(permissionGroups);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding permission groups");
            return false;
        }

        logger.LogInformation("Permission groups seeded successfully");
        return true;
    }
}
