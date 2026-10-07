using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Devices.WeighingMachines;

public static class WeighingMachinesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding weighing machines");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "weighingmachines.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Weighing machines seed file not found.");
            return false;
        }

        try
        {
            List<WeighingMachine>? weighingMachines = JsonSerializer.Deserialize<List<WeighingMachine>>(File.ReadAllText(seedFilePath));
            if (weighingMachines is null)
            {
                logger.LogError("Weighing machines seed file is empty.");
                return false;
            }
            database.WeighingMachines.AddRange(weighingMachines);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding weighing machines");
            return false;
        }

        logger.LogInformation("Weighing machines seeded successfully");
        return true;
    }
}
