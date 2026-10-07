using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Tables;

public static class TablesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding tables");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "tables.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Tables seed file not found.");
            return false;
        }

        try
        {
            List<Table>? tables = JsonSerializer.Deserialize<List<Table>>(File.ReadAllText(seedFilePath));
            if (tables is null)
            {
                logger.LogError("Tables seed file is empty.");
                return false;
            }
            database.Tables.AddRange(tables);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding tables");
            return false;
        }

        logger.LogInformation("Tables seeded successfully");
        return true;
    }
}
