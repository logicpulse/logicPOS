using System.Text.Json;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Devices.Printers.Types;

public static class PrinterTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding printer types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "printertypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Printer types seed file not found.");
            return false;
        }

        try
        {
            List<LogicPOS.Domain.Entities.PrinterType>? printerTypes = JsonSerializer.Deserialize<List<LogicPOS.Domain.Entities.PrinterType>>(File.ReadAllText(seedFilePath));
            if (printerTypes is null)
            {
                logger.LogError("Printer types seed file is empty.");
                return false;
            }
            database.PrinterTypes.AddRange(printerTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding printer types");
            return false;
        }

        logger.LogInformation("Printer types seeded successfully");
        return true;
    }
}
