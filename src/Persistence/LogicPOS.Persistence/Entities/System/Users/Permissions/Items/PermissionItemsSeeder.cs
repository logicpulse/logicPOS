using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Permissions.Items;

public static class PermissionItemsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding permission items");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "permissionitems.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Permission items seed file not found.");
            return false;
        }

        try
        {
            List<PermissionItem>? permissionItems = JsonSerializer.Deserialize<List<PermissionItem>>(File.ReadAllText(seedFilePath));
            if (permissionItems is null)
            {
                logger.LogError("Permission items seed file is empty.");
                return false;
            }
            database.PermissionItems.AddRange(permissionItems);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding permission items");
            return false;
        }

        logger.LogInformation("Permission items seeded successfully");
        return true;
    }
    
    public static bool SeedNewData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding new permission items");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "permissionitems.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Permission items seed file not found.");
            return false;
        }

        try
        {
            List<PermissionItem>? seedPermissionItems = JsonSerializer.Deserialize<List<PermissionItem>>(File.ReadAllText(seedFilePath));
            if (seedPermissionItems is null)
            {
                logger.LogError("Permission items seed file is empty.");
                return false;
            }
            
            string[] existentTokens = database.PermissionItems.Select(x => x.Token).ToArray();

            var newPermissionItems = seedPermissionItems.Where(x => !existentTokens.Contains(x.Token)).ToList();

            if (newPermissionItems.Count == 0)
            {
                logger.LogInformation("No new permission items detected");
                return true;
            }

            logger.LogInformation("Detected {NewPermissionItemsCount} new permission items: {Items}",
                newPermissionItems.Count, newPermissionItems.Select(x => x.Token).ToArray());
            
            database.PermissionItems.AddRange(newPermissionItems);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding new permission items");
            return false;
        }

        logger.LogInformation("New permission items seeded successfully");
        return true;
    }
}
