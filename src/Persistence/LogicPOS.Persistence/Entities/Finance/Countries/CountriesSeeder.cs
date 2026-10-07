using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Countries;

public static class CountriesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding countries");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "countries.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Countries seed file not found.");
            return false;
        }

        try
        {
            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(File.ReadAllText(seedFilePath));
            if (countries is null)
            {
                logger.LogError("Countries seed file is empty.");
                return false;
            }
            database.Countries.AddRange(countries);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding countries");
            return false;
        }
        
        logger.LogInformation("Countries seeded successfully");
        return true;
    }
}
