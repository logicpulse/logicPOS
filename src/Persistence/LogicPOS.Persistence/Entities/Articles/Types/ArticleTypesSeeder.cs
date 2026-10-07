using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Types;

public static class ArticleTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding article types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "articletypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogWarning("Article types seed file not found.");
            return false;
        }

        try
        {
            List<ArticleType>? articleTypes = JsonSerializer.Deserialize<List<ArticleType>>(File.ReadAllText(seedFilePath));
            if (articleTypes is null)
            {
                logger.LogWarning("Article types seed file is empty.");
                return false;
            }
            database.ArticleTypes.AddRange(articleTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding article types");
            return false;
        }
        
        logger.LogInformation("Article types seeded successfully");
        return true;
    }
}
