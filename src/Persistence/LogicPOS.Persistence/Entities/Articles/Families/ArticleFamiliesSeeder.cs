using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Families;

public static class ArticleFamiliesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding article families");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "articlefamilies.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Article families seed file not found.");
            return false;
        }

        try
        {
            List<ArticleFamily>? articleFamilies = JsonSerializer.Deserialize<List<ArticleFamily>>(File.ReadAllText(seedFilePath));
            if (articleFamilies is null)
            {
                logger.LogError("Article families seed file is empty.");
                return false;
            }
            database.ArticleFamilies.AddRange(articleFamilies);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding article families");
            return false;
        }
        
        logger.LogInformation("Article families seeded successfully");
        return true;
    }
}
