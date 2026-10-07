using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Stocks.Warehouses.Locations;

public static class WarehouseLocationsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding warehouse locations");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "warehouselocations.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Warehouse locations seed file not found.");
            return false;
        }

        try
        {
            List<WarehouseLocation>? warehouseLocations = JsonSerializer.Deserialize<List<WarehouseLocation>>(File.ReadAllText(seedFilePath));
            if (warehouseLocations is null)
            {
                logger.LogWarning("Warehouse locations seed file is empty.");
                return false;
            }
            database.WarehouseLocations.AddRange(warehouseLocations);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding warehouse locations");
            return false;
        }

        logger.LogInformation("Warehouse locations seeded successfully");
        return true;
    }
}
