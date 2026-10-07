using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Units.Size;

public static class SizeUnitsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding size units");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "sizeunits.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Size units seed file not found.");
            return false;
        }

        try
        {
            List<SizeUnit>? sizeUnits = JsonSerializer.Deserialize<List<SizeUnit>>(File.ReadAllText(seedFilePath));
            if (sizeUnits is null)
            {
                logger.LogError("Size units seed file is empty.");
                return false;
            }
            database.SizeUnits.AddRange(sizeUnits);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding size units");
            return false;
        }
        
        logger.LogInformation("Size units seeded successfully");
        return true;
    }
}
