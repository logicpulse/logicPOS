using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Permissions.Profiles;

public static class PermissionProfilesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding permission profiles");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "permissionprofiles.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Permission profiles seed file not found.");
            return false;
        }

        try
        {
            List<PermissionProfile>? permissionProfiles = JsonSerializer.Deserialize<List<PermissionProfile>>(File.ReadAllText(seedFilePath));
            if (permissionProfiles is null)
            {
                logger.LogError("Permission profiles seed file is empty.");
                return false;
            }
            database.PermissionProfiles.AddRange(permissionProfiles);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding permission profiles");
            return false;
        }

        logger.LogInformation("Permission profiles seeded successfully");
        return true;
    }
}
