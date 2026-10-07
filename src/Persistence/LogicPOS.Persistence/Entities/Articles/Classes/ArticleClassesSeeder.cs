using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Classes;

public static class ArticleClassesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding article classes");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "articleclasses.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Article classes seed file not found.");
            return false;
        }

        try
        {
            List<ArticleClass>? articleClasses = JsonSerializer.Deserialize<List<ArticleClass>>(File.ReadAllText(seedFilePath));
            if (articleClasses is null)
            {
                logger.LogWarning("Article classes seed file is empty.");
                return false;
            }
            database.ArticleClasses.AddRange(articleClasses);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding article classes");
            return false;
        }
        
        logger.LogInformation("Article classes seeded successfully");
        return true;
    }
}
