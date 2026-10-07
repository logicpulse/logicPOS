using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Users.Profiles;

public static class UserProfilesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding user profiles");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "userprofiles.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("User profiles seed file not found.");
            return false;
        }

        try
        {
            List<UserProfile>? userProfiles = JsonSerializer.Deserialize<List<UserProfile>>(File.ReadAllText(seedFilePath));
            if (userProfiles is null)
            {
                logger.LogError("User profiles seed file is empty.");
                return false;
            }
            database.UserProfiles.AddRange(userProfiles);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding user profiles");
            return false;
        }

        logger.LogInformation("User profiles seeded successfully");
        return true;
    }
}
