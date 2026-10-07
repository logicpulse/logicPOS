using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Notifications;

public static class SystemNotificationsTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings,  ILogger logger)
    {
        logger.LogInformation("Seeding system notifications types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "systemnotificationtypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("System notifications types seed file not found.");
            return false;
        }

        try
        {
            List<SystemNotificationType>? notificationTypes = JsonSerializer.Deserialize<List<SystemNotificationType>>(File.ReadAllText(seedFilePath));
            if (notificationTypes is null)
            {
                logger.LogError("System notifications types seed file is empty.");
                return false;
            }
            database.SystemNotificationTypes.AddRange(notificationTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding system notifications types");
            return false;
        }
        
        logger.LogInformation("System notifications types seeded successfully");
        
        return true;
    }
}
