using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.DiscountGroups;

public static class DiscountGroupsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding discount groups");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "discountgroups.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Discount groups seed file not found.");
            return false;
        }

        try
        {
            List<DiscountGroup>? discountGroups = JsonSerializer.Deserialize<List<DiscountGroup>>(File.ReadAllText(seedFilePath));
            if (discountGroups is null)
            {
                logger.LogError("Discount groups seed file is empty.");
                return false;
            }
            database.DiscountGroups.AddRange(discountGroups);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding discount groups");
            return false;
        }
        
        logger.LogInformation("Discount groups seeded successfully");
        return true;
    }
}
