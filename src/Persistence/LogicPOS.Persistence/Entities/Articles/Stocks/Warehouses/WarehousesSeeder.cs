using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Stocks.Warehouses;

public static class WarehousesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding warehouses");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "warehouses.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Warehouses seed file not found.");
            return false;
        }

        try
        {
            List<Warehouse>? warehouses = JsonSerializer.Deserialize<List<Warehouse>>(File.ReadAllText(seedFilePath));
            if (warehouses is null)
            {
                logger.LogError("Warehouses seed file is empty.");
                return false;
            }
            database.Warehouses.AddRange(warehouses);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding warehouses");
            return false;
        }
        
        logger.LogInformation("Warehouses seeded successfully");
        return true;
    }
}
