using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Devices.PoleDisplays;

public static class PoleDisplaysSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding pole displays");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "poledisplays.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Pole displays seed file not found.");
            return false;
        }

        try
        {
            List<PoleDisplay>? poleDisplays = JsonSerializer.Deserialize<List<PoleDisplay>>(File.ReadAllText(seedFilePath));
            if (poleDisplays is null)
            {
                logger.LogError("Pole displays seed file is empty.");
                return false;
            }
            database.PoleDisplays.AddRange(poleDisplays);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding pole displays");
            return false;
        }

        logger.LogInformation("Pole displays seeded successfully");
        return true;
    }
}
