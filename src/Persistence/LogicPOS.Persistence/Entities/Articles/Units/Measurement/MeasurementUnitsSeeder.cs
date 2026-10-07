using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Units.Measurement;

public static class MeasurementUnitsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding measurement units");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "measurementunits.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Measurement units seed file not found.");
            return false;
        }

        try
        {
            List<MeasurementUnit>? measurementUnits = JsonSerializer.Deserialize<List<MeasurementUnit>>(File.ReadAllText(seedFilePath));
            if (measurementUnits is null)
            {
                logger.LogError("Measurement units seed file is empty.");
                return false;
            }
            database.MeasurementUnits.AddRange(measurementUnits);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding measurement units");
            return false;
        }
        
        logger.LogInformation("Measurement units seeded successfully");
        return true;
    }
}
