using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.PriceTypes;

public static class PriceTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding price types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "pricetypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Price types seed file not found.");
            return false;
        }

        try
        {
            List<PriceType>? priceTypes = JsonSerializer.Deserialize<List<PriceType>>(File.ReadAllText(seedFilePath));
            if (priceTypes is null)
            {
                logger.LogError("Price types seed file is empty.");
                return false;
            }
            database.PriceTypes.AddRange(priceTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding price types");
            return false;
        }
        
        logger.LogInformation("Price types seeded successfully");
        return true;
    }
}
