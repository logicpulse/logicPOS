using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Articles;

public static class ArticlesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding articles");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "articles.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Articles seed file({Path}) not found.", seedFilePath);
            return false;
        }

        try
        {
            List<Article>? articles = JsonSerializer.Deserialize<List<Article>>(File.ReadAllText(seedFilePath));
            if (articles is null)
            {
                logger.LogError("Articles seed file is empty.");
                return false;
            }
            database.Articles.AddRange(articles);
            database.SaveChanges();
            
        } catch (Exception e)
        {
            logger.LogError(e, "Error while seeding articles");
            return false;
        }

        logger.LogInformation("Articles seeded successfully");
        return true;
    }
}