using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Holidays;

public static class HolidaysSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2, ILogger logger)
    {
        logger.LogInformation("Seeding holidays");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2,"holidays.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Holidays seed file not found.");
            return false;
        }

        try
        {
            List<Holiday>? holidays = JsonSerializer.Deserialize<List<Holiday>>(File.ReadAllText(seedFilePath));
            if (holidays is null)
            {
                logger.LogError("Holidays seed file is empty.");
                return false;
            }
            database.Holidays.AddRange(holidays);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding holidays");
            return false;
        }
        
        logger.LogInformation("Holidays seeded successfully");
        return true;
    }
}