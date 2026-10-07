using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Subfamilies;

public static class ArticleSubfamiliesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding article subfamilies");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "articlesubfamilies.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Article subfamilies seed file not found.");
            return false;
        }

        try
        {
            List<ArticleSubfamily>? articleSubfamilies = JsonSerializer.Deserialize<List<ArticleSubfamily>>(File.ReadAllText(seedFilePath));
            if (articleSubfamilies is null)
            {
                logger.LogError("Article subfamilies seed file is empty.");
                return false;
            }
            database.ArticleSubfamilies.AddRange(articleSubfamilies);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding article subfamilies");
            return false;
        }
        
        logger.LogInformation("Article subfamilies seeded successfully");
        return true;
    }
}
