using System.Text.Json;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Devices.Printers.Printers;

public static class PrintersSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding printers");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "printers.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Printers seed file not found.");
            return false;
        }

        try
        {
            List<LogicPOS.Domain.Entities.Printer>? printers = JsonSerializer.Deserialize<List<LogicPOS.Domain.Entities.Printer>>(File.ReadAllText(seedFilePath));
            if (printers is null)
            {
                logger.LogError("Printers seed file is empty.");
                return false;
            }
            database.Printers.AddRange(printers);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding printers");
            return false;
        }

        logger.LogInformation("Printers seeded successfully");
        return true;
    }
}