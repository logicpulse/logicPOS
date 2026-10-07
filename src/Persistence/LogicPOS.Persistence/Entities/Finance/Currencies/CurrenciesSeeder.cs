using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Currencies;

public static class CurrenciesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding currencies");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "currencies.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Currencies seed file not found.");
            return false;
        }

        try
        {
            List<Currency>? currencies = JsonSerializer.Deserialize<List<Currency>>(File.ReadAllText(seedFilePath));
            if (currencies is null)
            {
                logger.LogError("Currencies seed file is empty.");
                return false;
            }
            database.Currencies.AddRange(currencies);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding currencies");
            return false;
        }
        
        logger.LogInformation("Currencies seeded successfully");
        return true;
    }
}
