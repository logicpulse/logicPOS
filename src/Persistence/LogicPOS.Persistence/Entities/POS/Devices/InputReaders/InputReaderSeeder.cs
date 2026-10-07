using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.POS.Devices.InputReaders;

public static class InputReaderSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding input readers");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "inputreaders.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Input readers seed file not found.");
            return false;
        }

        try
        {
            List<InputReader>? inputReaders = JsonSerializer.Deserialize<List<InputReader>>(File.ReadAllText(seedFilePath));
            if (inputReaders is null)
            {
                logger.LogError("Input readers seed file is empty.");
                return false;
            }
            database.InputReaders.AddRange(inputReaders);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding input readers");
            return false;
        }

        logger.LogInformation("Input readers seeded successfully");
        return true;
    }
}
