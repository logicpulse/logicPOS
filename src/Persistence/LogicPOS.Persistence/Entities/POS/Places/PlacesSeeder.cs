using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Places;

public static class PlacesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding places");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "places.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Places seed file not found.");
            return false;
        }

        try
        {
            List<Place>? places = JsonSerializer.Deserialize<List<Place>>(File.ReadAllText(seedFilePath));
            if (places is null)
            {
                logger.LogError("Places seed file is empty.");
                return false;
            }
            database.Places.AddRange(places);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding places");
            return false;
        }

        logger.LogInformation("Places seeded successfully");
        return true;
    }
}
