using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.MovementTypes;

public static class MovementTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding movement types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "movementtypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Movement types seed file not found.");
            return false;
        }

        try
        {
            List<MovementType>? movementTypes = JsonSerializer.Deserialize<List<MovementType>>(File.ReadAllText(seedFilePath));
            if (movementTypes is null)
            {
                logger.LogError("Movement types seed file is empty.");
                return false;
            }
            database.MovementTypes.AddRange(movementTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding movement types");
            return false;
        }

        logger.LogInformation("Movement types seeded successfully");
        return true;
    }
}
