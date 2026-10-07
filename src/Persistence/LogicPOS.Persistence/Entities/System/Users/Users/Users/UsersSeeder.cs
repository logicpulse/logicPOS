using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Users.Users;

public static class UsersSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding users");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "users.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Users seed file not found.");
            return false;
        }

        try
        {
            List<User>? users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(seedFilePath));
            if (users is null)
            {
                logger.LogError("Users seed file is empty.");
                return false;
            }
            database.Users.AddRange(users);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding users");
            return false;
        }

        logger.LogInformation("Users seeded successfully");
        return true;
    }
}
